using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpellChecker.Test {
   [TestClass]
   public class TestSpellChecker {
      [TestMethod]
      public void TestSpellChecks() {
         using (var queuedSpellChecker = new QueuedSpellChecker()) {
            var result = queuedSpellChecker.PerformSpellCheck("Spelling mispake.", 0, new List<String>());
            Assert.AreEqual(9, result.StartIndex);
            Assert.AreEqual(7, result.Length);
            Assert.AreEqual("mistake", result.Suggestions[0]);
         }
      }

      [TestMethod]
      public void TestSpellCheckUsingCorrectEnglish() {
         using (var queuedSpellChecker = new QueuedSpellChecker(true)) {
            var result = queuedSpellChecker.PerformSpellCheck("Color.", 0, new List<String>());
            Assert.AreEqual(0, result.StartIndex);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("Colour", result.Suggestions[0]);
         }
      }

      [TestMethod]
      public void TestSpellCheckUsingIncorrectEnglish() {
         using (var queuedSpellChecker = new QueuedSpellChecker(false)) {
            var result = queuedSpellChecker.PerformSpellCheck("Color.", 0, new List<String>());
            Assert.IsNull(result);
         }
      }

      [TestMethod]
      //I've left this test here just to see how performant the spell check is
      //and to show an example of using it outside a using block
      public void TestAFairFewSpellChecks() {
         var sw = Stopwatch.StartNew();

         var queuedSpellChecker = new QueuedSpellChecker();
         for (int i = 0; i < 100000; i++) {
            queuedSpellChecker.PerformSpellCheck("Helllp", 0, new List<String>());
         }

         sw.Stop();
         Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);

         queuedSpellChecker.Dispose();
      }
   }
}
