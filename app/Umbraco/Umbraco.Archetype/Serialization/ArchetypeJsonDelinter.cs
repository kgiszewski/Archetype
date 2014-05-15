using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Archetype.Umbraco.Serialization
{
    public enum DelinterStep
    {
        RemoveNewLine = 0,
        LabelFieldsets,
        LabelProperties,
        LabelAlias,
        LabelValue,
        UnescapeValues,
        FixNestedFieldsets,
        RemoveWhiteSpace
    }

    public enum DelinterAction
    {
        Remove = 0,
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
                {DelinterStep.RemoveNewLine, new Regex(@"\\r|\\n|[\r\n]", RegexOptions.Multiline)},
                {DelinterStep.LabelFieldsets, new Regex(@"\s*\\+""(fieldsets)\\+"":\s*")},
                {DelinterStep.LabelProperties, new Regex(@"\s*\\+""(properties)\\+"":\s*")},
                {DelinterStep.LabelAlias, new Regex(@"\s*\\+""(alias)\\+"":\s*")},
                {DelinterStep.LabelValue, new Regex(@"\s*\\+""(value)\\+"":\s*")},
                {DelinterStep.UnescapeValues, new Regex(@"""(alias|value)"":\\+""(.*?)\\+""")},
                {DelinterStep.FixNestedFieldsets, new Regex(@"""(value)"":\s*?""({""fieldsets"".+?})""")},
                {DelinterStep.RemoveWhiteSpace, new Regex(@"\s+(?=([^""]*""[^""]*"")*[^""]*$)")}
            };
            
            Actions = new Dictionary<DelinterAction, Func<Match, string>>
            {
                {DelinterAction.Remove, match => String.Empty},
                {DelinterAction.UnescapeLabels, match => String.Format(@"""{0}"":", match.Groups[1])},
                {DelinterAction.UnescapeValues, match => String.Format(@"""{0}"":""{1}""", match.Groups[1], match.Groups[2])},
                {DelinterAction.FixNestedFieldsets, match => String.Format(@"""{0}"":{1}", match.Groups[1], match.Groups[2])}
            };
            
            Pipeline = new Dictionary<DelinterStep, DelinterAction>
            {
                {DelinterStep.RemoveNewLine, DelinterAction.Remove},
                {DelinterStep.LabelFieldsets, DelinterAction.UnescapeLabels},
                {DelinterStep.LabelProperties, DelinterAction.UnescapeLabels},
                {DelinterStep.LabelAlias, DelinterAction.UnescapeLabels},
                {DelinterStep.LabelValue, DelinterAction.UnescapeLabels},
                {DelinterStep.UnescapeValues, DelinterAction.UnescapeValues},
                {DelinterStep.FixNestedFieldsets, DelinterAction.FixNestedFieldsets},
                {DelinterStep.RemoveWhiteSpace, DelinterAction.Remove}
            };
        }
    }
}
