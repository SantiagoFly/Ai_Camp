using Camposol.ViewModels;

namespace Camposol.Pages;

/// <summary>
/// Login UI
/// </summary>
public partial class LoginPage
{

	/// <summary>
	/// Receives the depedencies by DI
	/// </summary>
	public LoginPage(LoginViewModel viewModel) : base(viewModel, "Login")
	{
		InitializeComponent();
	}
}

