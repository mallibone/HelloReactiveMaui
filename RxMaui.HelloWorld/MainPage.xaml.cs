using ReactiveUI;
using ReactiveUI.Maui;

namespace RxMaui.HelloWorld;

public partial class MainPage : ReactiveContentPage<MainViewModel>
{
	public MainPage(MainViewModel viewModel)
	{
		ViewModel = viewModel;
		InitializeComponent();
		this.WhenActivated(_ => { });
	}
}
