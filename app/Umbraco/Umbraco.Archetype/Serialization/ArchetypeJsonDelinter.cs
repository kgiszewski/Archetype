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
        public IDictionary<DelinterStep, DelinterAction> Pipeline { get; set; }
        public IDictionary<DelinterStep, Regex> Tokens { get; set; }
        public IDictionary<DelinterAction, Func<Match,string>> Actions { get; set; }

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
                while (Tokens[step].IsMatch(buffer))
                {
                    buffer = Tokens[step]
                        .Replace(buffer, match => Actions[action](match));
                }                
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
                {DelinterStep.FixNestedFieldsets, new Regex(@"""(value)"":\s*?""\{\s*?(""fieldsets""[\S\s]+?)}""")}
            };
            
            Actions = new Dictionary<DelinterAction, Func<Match, string>>
            {
                {DelinterAction.Remove, match => String.Empty},
                {DelinterAction.RemoveWhiteSpace, match => match.Groups[2].Success ? match.Groups[0].Value : String.Empty},
                {DelinterAction.UnescapeLabels, match => String.Format(@"""{0}"":", match.Groups[1].Value)},
                {DelinterAction.UnescapeValues, match => String.Format(@"""{0}"":""{1}""", match.Groups[1].Value, match.Groups[2].Value)},
                {DelinterAction.FixNestedFieldsets, match => String.Format(@"""{0}"":{{{1}}}", match.Groups[1].Value, match.Groups[2].Value)}
            };
            
            Pipeline = new Dictionary<DelinterStep, DelinterAction>
            {
                {DelinterStep.RemoveNewLine, DelinterAction.Remove},                    
                //{DelinterStep.RemoveWhiteSpace, DelinterAction.RemoveWhiteSpace},                            
                {DelinterStep.UnescapeLabels, DelinterAction.UnescapeLabels},
                {DelinterStep.UnescapeAlias, DelinterAction.UnescapeValues},
                {DelinterStep.UnescapeValues, DelinterAction.UnescapeValues},
                {DelinterStep.FixNestedFieldsets, DelinterAction.FixNestedFieldsets}
            };
        }
    }
}
