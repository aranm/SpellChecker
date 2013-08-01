using System.Windows.Controls;
using System.Windows.Markup;

namespace SpellChecker {
   public class SpellCheckTextBox : TextBox {
      public SpellCheckTextBox() {
         this.SpellCheck.IsEnabled = true;
      }

      // ReSharper disable EmptyGeneralCatchClause
      public SpellCheckTextBox(bool useNonBastardisedEnglish)
         : this(){
         //try to use a version of the english language that does not include words like 'color'
         if (useNonBastardisedEnglish) {
            //try to set the language to AU or GB
            try {
               this.Language = XmlLanguage.GetLanguage("en-GB");
            }
            catch {
               try {
                  this.Language = XmlLanguage.GetLanguage("en-AU");
               }
               catch {}
            }
         }
         else {
            try {
               this.Language = XmlLanguage.GetLanguage("en-US");
            }
            catch { }
         }
      }
      // ReSharper restore EmptyGeneralCatchClause
   }
}
