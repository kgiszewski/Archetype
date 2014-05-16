using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Archetype.Serialization
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
                {DelinterStep.RemoveNewLine, new Regex(@"(?<!""value"":"".*?\\r\\n)\\r\\n", RegexOptions.Multiline)},
                {DelinterStep.LabelFieldsets, new Regex(@"\s*\\+""(fieldsets)\\+"":\s*", RegexOptions.Multiline)},
                {DelinterStep.LabelProperties, new Regex(@"\s*\\+""(properties)\\+"":\s*", RegexOptions.Multiline)},
                {DelinterStep.LabelAlias, new Regex(@"\s*\\+""(alias)\\+"":\s*", RegexOptions.Multiline)},
                {DelinterStep.LabelValue, new Regex(@"\s*\\+""(value)\\+"":\s*", RegexOptions.Multiline)},
                {DelinterStep.UnescapeValues, new Regex(@"""(alias|value)"":\\+""(.*?)\\+""", RegexOptions.Multiline)},
                {DelinterStep.FixNestedFieldsets, new Regex(@"""(value)"":\s*?""{[\\rn]+?(""fieldsets""[\S\s]+?)}""", RegexOptions.Multiline)},
                {DelinterStep.RemoveWhiteSpace, new Regex(@"\s+(?=([^""]*""[^""]*"")*[^""]*$)")}
            };
            
            Actions = new Dictionary<DelinterAction, Func<Match, string>>
            {
                {DelinterAction.Remove, match => String.Empty},
                {DelinterAction.UnescapeLabels, match => String.Format(@"""{0}"":", match.Groups[1])},
                {DelinterAction.UnescapeValues, match => String.Format(@"""{0}"":""{1}""", match.Groups[1], match.Groups[2])},
                {DelinterAction.FixNestedFieldsets, match => String.Format(@"""{0}"":{{{1}}}", match.Groups[1], match.Groups[2])}
            };
            
            Pipeline = new Dictionary<DelinterStep, DelinterAction>
            {
                {DelinterStep.LabelFieldsets, DelinterAction.UnescapeLabels},
                {DelinterStep.LabelProperties, DelinterAction.UnescapeLabels},
                {DelinterStep.LabelAlias, DelinterAction.UnescapeLabels},
                {DelinterStep.LabelValue, DelinterAction.UnescapeLabels},
                {DelinterStep.UnescapeValues, DelinterAction.UnescapeValues},
                {DelinterStep.FixNestedFieldsets, DelinterAction.FixNestedFieldsets},
                {DelinterStep.RemoveWhiteSpace, DelinterAction.Remove},
                {DelinterStep.RemoveNewLine, DelinterAction.Remove}
            };
        }
    }
}
