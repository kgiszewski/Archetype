using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Archetype.Serialization
{
    public enum DelinterStep
    {
        RemoveWhiteSpace = 0,        
        RemoveNewLine,
        UnescapeLabels,
        UnescapeAlias,
        UnescapeValues,
        FixNestedFieldsets,
        FixValueContent
    }

    public enum DelinterAction
    {
        Remove = 0,
        RemoveWhiteSpace,
        UnescapeLabels,
        UnescapeValues,
        FixNestedFieldsets,
        FixValueContent
    }
    
    public class ArchetypeJsonDelinter
    {
        public IDictionary<DelinterStep, DelinterAction> Pipeline { get; set; }
        public IDictionary<DelinterStep, Regex> Tokens { get; set; }
        public IDictionary<DelinterAction, Func<string, Regex, string>> Actions { get; set; }

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
                {DelinterStep.RemoveNewLine, new Regex(@"(?<=\,){0,1}(\\+r\\+n)(?=\\*?""""(alias|value|properties|fieldsets)\\*?"""":\{*?(\\*|""""|\[))|(\\+r\\+n)(?=\s*?(\{|\}|\]))|(\r|\n)+|(\\+r\\n+)(?!\s+.*?\\+""})")},                
                {DelinterStep.RemoveWhiteSpace, new Regex(@"(\s*?)\\+""(fieldsets|properties|alias|value)\\+"":(\s+)|""(\s*?)\},(\s+)\{|\[(\s+)\{|[\]\}](\s+)[\]\}]")},
                {DelinterStep.UnescapeLabels, new Regex(@"\\+""(fieldsets|properties|alias|value)\\+"":(\s*)")},
                {DelinterStep.UnescapeAlias, new Regex(@"""(alias)"":\\+""(.*?)\\+""")},
                {DelinterStep.UnescapeValues, new Regex(@"""(value)"":\\+""(.*?)\\+""(?=\s*?\})")},
                {DelinterStep.FixNestedFieldsets, new Regex(@"""(value)"":\s*?""\{\s*?(""fieldsets""[\S\s]+?)}""")},
                {DelinterStep.FixValueContent, new Regex(@"""(value)"":""(.*?)(?<!\\)""")}
            };

            Actions = new Dictionary<DelinterAction, Func<string, Regex, string>>
            {
                {DelinterAction.Remove, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Empty)
                },
                {DelinterAction.RemoveWhiteSpace, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => match.Groups[2].Success ? match.Groups[0].Value : String.Empty)
                },
                {DelinterAction.UnescapeLabels, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Format(@"""{0}"":", match.Groups[1].Value))
                },
                {DelinterAction.UnescapeValues, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Format(@"""{0}"":""{1}""", match.Groups[1].Value, match.Groups[2].Value))
                },
                {DelinterAction.FixNestedFieldsets, (input, pattern) =>
                    RecursiveReplace(input, pattern, match => String.Format(@"""{0}"":{{{1}}}", match.Groups[1].Value, match.Groups[2].Value))
                },
                {DelinterAction.FixValueContent, (input, pattern) =>
                    pattern.Replace(input, match => String.Format(@"""{0}"":""{1}""", match.Groups[1].Value, 
                    (new Regex(@"\\+")).Replace(match.Groups[2].Value, @"\")))
                },
            };
            
            Pipeline = new Dictionary<DelinterStep, DelinterAction>
            {
                {DelinterStep.RemoveNewLine, DelinterAction.Remove},                                              
                {DelinterStep.UnescapeLabels, DelinterAction.UnescapeLabels},
                {DelinterStep.UnescapeAlias, DelinterAction.UnescapeValues},
                {DelinterStep.UnescapeValues, DelinterAction.UnescapeValues},
                {DelinterStep.FixNestedFieldsets, DelinterAction.FixNestedFieldsets},
                {DelinterStep.FixValueContent, DelinterAction.FixValueContent}
                //{DelinterStep.RemoveWhiteSpace, DelinterAction.RemoveWhiteSpace},  
            };
        }

        private string RecursiveReplace(string input, Regex pattern, Func<Match, string> matchFunc)
        {
            var buffer = input;
            while (pattern.IsMatch(buffer))
            {
                buffer = pattern.Replace(input, match => matchFunc(match));
            }
            return buffer;            
        }
    }
}
