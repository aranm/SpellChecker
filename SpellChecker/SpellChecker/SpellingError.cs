using System.Collections.Generic;

namespace SpellChecker {
   public class SpellingError {
      public int StartIndex { get; set; }
      public int Length { get; set; }
      public List<string> Suggestions { get; set; }
   }
}