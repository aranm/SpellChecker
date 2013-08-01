using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SpellChecker {
   public class QueuedSpellChecker : IDisposable {
      private SpellCheckProvider _spellChecker;
      private readonly Thread _thread;
      private Dispatcher _dispatcher;

      public QueuedSpellChecker() 
         : this(false) { }

      public QueuedSpellChecker(bool useCorrectEnglish) {

         //we want to make sure the dispatcher is fired up before we finish execution of the constructor
         var dispatcherReadyEvent = new ManualResetEvent(false);

         _thread = new Thread(() => {
            _spellChecker = new SpellCheckProvider(useCorrectEnglish);
            _dispatcher = Dispatcher.CurrentDispatcher;
            dispatcherReadyEvent.Set();

            Dispatcher.Run();
         });

         _thread.SetApartmentState(ApartmentState.STA);
         _thread.Start();

         dispatcherReadyEvent.WaitOne();
      }

      /// <summary>
      /// Perform a spell check on the supplied text starting from the start index and ignoring
      /// spelling error on any words that are passed in
      /// </summary>
      /// <param name="text">The text to spell check</param>
      /// <param name="startIndex">The index to start from</param>
      /// <param name="ignoredWords">A list of words to ignore when performing the spell check</param>
      /// <returns></returns>
      public SpellingError PerformSpellCheck(string text, int startIndex, List<string> ignoredWords) {
         //schedule a task on the dispatcher, wpf controls require this context in order to run correctly
         Func<SpellingError> nfunc = () => {
            var spellingMistake = _spellChecker.CheckSpelling(text, startIndex, ignoredWords);
            if (spellingMistake == null) {
               return null;
            }
            else {
               return new SpellingError {
                  Length = spellingMistake.Length,
                  StartIndex = spellingMistake.StartIndex,
                  Suggestions = spellingMistake.Suggestions
               };
            }
         };

         var task = ScheduleSpellCheck(nfunc);
         var result = task.Result;
         task.Dispose();
         return result;
      }

      private Task<SpellingError> ScheduleSpellCheck(Func<SpellingError> func) {
         var taskSchedulerAsync = _dispatcher.ToTaskSchedulerAsync();
         var taskFactoryAsync = taskSchedulerAsync.ContinueWith(_ =>
             new TaskFactory(taskSchedulerAsync.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
         var taskFactory = taskFactoryAsync.Result;
         var task = taskFactory.StartNew(func);
         return task;
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      /// <filterpriority>2</filterpriority>
      public void Dispose() {
         _dispatcher.InvokeShutdown();
         _thread.Join();
      }
   }
}
