SpellChecker
============

A free .NET Spell checker. Spell checkers are expensive and most come tied to a UI that may or may not suit your application. This spell checker was developed to run a server side spell check for a web application. It performs pretty well.


How to use the spell checker
---
There are a few ways to use the spell checker, but the usage is very simple and straightforward:

```csharp
using (var queuedSpellChecker = new QueuedSpellChecker()) {
   var result = queuedSpellChecker.PerformSpellCheck("Spelling mispake.", 0, new List<String>());
   Assert.AreEqual(9, result.StartIndex);
   Assert.AreEqual(7, result.Length);
   Assert.AreEqual("mistake", result.Suggestions[0]);
}
```

The spell checker uses a WPF text box under the hood to perform the spell checking. 
In order for this to function correctly you need to run all of the spell checks on a dispatcher thread. 
This is handled for you in the `QueuedSpellChecker` class. 
If you are running a WPF application and don't mind doing your spell check on the foreground thread you can just use the `SpellCheckProvider` directly like so:

```csharp
var spellCheckProvider = new SpellCheckProvider();
var result = spellCheckProvider.CheckSpelling("Spelling mispake.", 0, new List<String>());
```

See the unit tests (of which there is never enough) for some more examples of usage.

The Queens English
---

Down with words like *Color*. Be proud of the firm tradition of excellence in language, don't let those heathen on the other side of the Atlantic destroy a beautiful thing.
Pass a `Boolean` to only use the *British* (or Australian) English dictionary.

```csharp
[TestMethod]
public void TestSpellCheckUsingCorrectEnglish() {
   //Pass true to use correct English, otherwise the spell checker defaults to the language settings on the local machine
   using (var queuedSpellChecker = new QueuedSpellChecker(true)) {
      var result = queuedSpellChecker.PerformSpellCheck("Color.", 0, new List<String>());
      Assert.AreEqual(0, result.StartIndex);
      Assert.AreEqual(5, result.Length);
      Assert.AreEqual("Colour", result.Suggestions[0]);
   }
}
```

Caveats...the catch
--

You need to include references to PresentationFramework and associated assemblies in order to use this (as it relies on WPF)