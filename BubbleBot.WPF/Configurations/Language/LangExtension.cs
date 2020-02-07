using System;
using System.Windows.Markup;

namespace BubbleBot.Configurations.Language
{
    public class LangExtension : MarkupExtension
    {

        // Properties
        [ConstructorArgument("key")]
        public string Key { get; set; }


        // Constructor
        public LangExtension(string key)
        {
            Key = key;
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
            => LanguageManager.Translate(Key);

    }
}
