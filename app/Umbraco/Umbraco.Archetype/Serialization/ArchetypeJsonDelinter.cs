using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Archetype.Serialization
{
    public enum DelinterStep
    {     
        RemoveNewLine = 0,
        RemoveWhiteSpace,
        UnescapeLabels,
        UnescapeAlias,
        UnescapeValues,
        FixNestedFieldsets
    }

    public enum DelinterAction
    {
        Remove = 0,
        RemoveWhiteSpace,
        UnescapeLabels,
        UnescapeValues,
        FixNestedFieldsets
    }
    
    public class ArchetypeJsonDelinter
    {
        public IDictionary<DelinterStep, DelinterAction> Pipeline { get; private set; }
        public IDictionary<DelinterStep, Regex> Tokens { get; private set; }
        public IDictionary<DelinterAction, Func<string, Regex, string>> Actions { get; private set; }

        public ArchetypeJsonDelinter()
        {
            Init();
        }

        public string Execute(string input)
        {
            var buffer = input;

            foreach (var step in Pipeline.Keys)
            {
                var action = Pipeline[step];
                buffer = Actions[action](buffer, Tokens[step]);             
            }

            return buffer;
        }

        private void Init()
        {
            Tokens = new Dictionary<DelinterStep, Regex>
            {
                {DelinterStep.RemoveNewLine, new Regex(@"(?<=\,){0,1}(\\+r\\+n)(?=\\*?""(alias|value|properties|fieldsets)\\*?"":\{*?(\\*|""|\[))|(\r|\n)+|(\\+r\\+n)(?=\s*?(\{|\}|\]))|(\\+r\\n+)(?=\s+\\*?""(alias|value|properties|fieldsets))")},                
                {DelinterStep.RemoveWhiteSpace, new Regex(@"(""\S+?"":)(\s+?)("")|([\{\[\}\],]|(?<!\\)"")(\s+)([\{\[\}\],]|(?<!\\)"")")},
                {DelinterStep.UnescapeLabels, new Regex(@"\\+""(fieldsets|properties|alias|value)\\+"":(\s*)")},
                {DelinterStep.UnescapeAlias, new Regex(@"""(alias)"":\\+""(.*?)\\+""")},
                {DelinterStep.UnescapeValues, new Regex(@"""(value)"":\\+""(.*?)\\+""(?=\s*?\})")},
                {DelinterStep.FixNestedFieldsets, new Regex(@"""(value)"":\s*?""\{\s*?(""fieldsets""[\S\s]+?)}""")}
            };

            Actions = new Dictionary<DelinterAction, Func<string, Regex, string>>
            {
                {DelinterAction.Remove, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Empty)
                },
                {DelinterAction.RemoveWhiteSpace, (input, pattern) =>
                    RecursiveReplace(input.Trim(), pattern, match =>
                        String.Format("{0}{1}{2}{3}", match.Groups[1].Value, 
                            match.Groups[3].Value, match.Groups[4].Value, match.Groups[6].Value))
                },
                {DelinterAction.UnescapeLabels, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Format(@"""{0}"":", match.Groups[1].Value))
                },
                {DelinterAction.UnescapeValues, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Format(@"""{0}"":""{1}""", match.Groups[1].Value, match.Groups[2].Value))
                },
                {DelinterAction.FixNestedFieldsets, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Format(@"""{0}"":{{{1}}}", match.Groups[1].Value, match.Groups[2].Value))
                }
            };
            
            Pipeline = new Dictionary<DelinterStep, DelinterAction>
            {
                {DelinterStep.RemoveNewLine, DelinterAction.Remove},                                              
                {DelinterStep.UnescapeLabels, DelinterAction.UnescapeLabels},
                {DelinterStep.UnescapeAlias, DelinterAction.UnescapeValues},
                {DelinterStep.UnescapeValues, DelinterAction.UnescapeValues},
                {DelinterStep.FixNestedFieldsets, DelinterAction.FixNestedFieldsets},
                {DelinterStep.RemoveWhiteSpace, DelinterAction.RemoveWhiteSpace},  
            };
        }

        private string RecursiveReplace(string input, Regex pattern, Func<Match, string> matchFunc)
        {
            var buffer = input;
            while (pattern.IsMatch(buffer))
            {
                buffer = pattern.Replace(buffer, match => matchFunc(match));
            }
            return buffer;            
        }
    }
}
