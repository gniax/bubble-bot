using System;
using System.Windows.Markup;

namespace BubbleBot.Configurations.Language
{
    public class LangExtension : MarkupExtension
    {
        // Constructor
        public LangExtension(string key)
        {
            Key = key;
        }

        // Properties
        [ConstructorArgument("key")] public string Key { get; set; }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return LanguageManager.Translate(Key);
        }
    }
}