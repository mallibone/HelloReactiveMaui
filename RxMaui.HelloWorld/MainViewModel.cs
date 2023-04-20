using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RxMaui.HelloWorld;

public class MainViewModel : ReactiveObject, IActivatableViewModel
{
	private static readonly string[] Traits = {
		"expressive",
		"clear",
		"responsive",
		"concurrent",
		"reactive"
	};

	public MainViewModel()
	{
		ButtonClickedCommand = ReactiveCommand.Create(ButtonClicked);
	    
		// Create the text for the counterbutton
		this.WhenAnyValue(vm => vm.Count)
			.Select(c => c switch
			{
				0 => "Click me",
				1 => "Clicked 1 time",
				_ => $"Clicked {c} times"
			})
			.ToPropertyEx(this, vm => vm.CounterButtonText);

		this.WhenActivated(disposables =>
		{
			// Just log the ViewModel's activation
			// https://github.com/kentcb/YouIandReactiveUI/blob/master/ViewModels/Samples/Chapter%2018/Sample%2004/ChildViewModel.cs
			Console.WriteLine(
				$"[vm {Thread.CurrentThread.ManagedThreadId}]: " +
				"ViewModel activated");
		    
			// Asynchronously generate a new greeting message every second
			// https://reactiveui.net/docs/guidelines/framework/ui-thread-and-schedulers
			Observable
				.Timer(
					TimeSpan.FromMilliseconds(100), // give the view time to activate
					TimeSpan.FromMilliseconds(1000))
				.Do(
					t => {
						var newGreeting = $"Hello, {Traits[t % Traits.Length]} world !";
						Console.WriteLine(
							$"[vm {Thread.CurrentThread.ManagedThreadId}]: " +
							$"Timer Observable -> " +
							$"Setting greeting to: \"{newGreeting}\"");
						Greeting = newGreeting;
					},
					() => 
						Console.WriteLine(
							"Those are all the greetings, folks! " +
							"Feel free to close the window now...\n"))
				.Subscribe()
				.DisposeWith(disposables);

			// Just log the ViewModel's deactivation
			// https://github.com/kentcb/YouIandReactiveUI/blob/master/ViewModels/Samples/Chapter%2018/Sample%2004/ChildViewModel.cs
			Disposable
				.Create(
					() =>
						Console.WriteLine(
							$"[vm {Thread.CurrentThread.ManagedThreadId}]: " +
							"ViewModel deactivated"))
				.DisposeWith(disposables);
		});
	}

	[Reactive] public string Greeting { get; set; }
    
	[Reactive] public int Count { get; set; }
	[ObservableAsProperty] public string CounterButtonText { get; }
	public ReactiveCommand<Unit,Unit> ButtonClickedCommand { get; }

	public ViewModelActivator Activator { get; } = new();

	private void ButtonClicked() => Count++;
}

