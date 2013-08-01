using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace SpellChecker {

   public class SpellCheckProvider{
      private readonly SpellCheckTextBox _spellCheckTextBox;

      public SpellCheckProvider() {
         _spellCheckTextBox = new SpellCheckTextBox();
      }

      public SpellCheckProvider(bool useCorrectEnglish) {
         _spellCheckTextBox = new SpellCheckTextBox(useCorrectEnglish);
      }

      #region ISpellCheck Members

      public SpellingError CheckSpelling(string text, List<String> ignoredWords) {
         return CheckSpelling(text, 0, ignoredWords);
      }

      public SpellingError CheckSpelling(string text, int startPosition, List<String> ignoredWords) {

         SpellingError spellingError = null;

         _spellCheckTextBox.Text = text;

         var finished = false;

         while (finished == false) {

            int errorIndex = _spellCheckTextBox.GetNextSpellingErrorCharacterIndex(startPosition, LogicalDirection.Forward);

            //we have an error
            if (errorIndex < 0) {
               finished = true;
            }
            else {
               spellingError = new SpellingError {
                  StartIndex = errorIndex,
                  Length = _spellCheckTextBox.GetSpellingErrorLength(errorIndex)
               };

               var currentSpellingError = text.Substring(spellingError.StartIndex, spellingError.Length);
               if (ignoredWords != null && ignoredWords.Any(item => String.Compare(item, currentSpellingError, StringComparison.OrdinalIgnoreCase) == 0)) {
                  startPosition = spellingError.StartIndex + spellingError.Length;
               }
               else {
                  var error = _spellCheckTextBox.GetSpellingError(errorIndex);
                  if (error != null && error.Suggestions != null) {
                     spellingError.Suggestions = new List<string>(error.Suggestions);
                  }
                  finished = true;
               }
            }

         }
         return spellingError;
      }

      #endregion
   }
}
