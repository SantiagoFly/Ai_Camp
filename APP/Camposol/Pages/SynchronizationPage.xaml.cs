﻿
using Camposol.ViewModels;

namespace Camposol.Pages;

/// <summary>
/// Synchronization UI
/// </summary>
public partial class SynchronizationPage
{

	/// <summary>
	/// Receives the depedencies by DI
	/// </summary>
	public SynchronizationPage(SynchronizationViewModel viewModel) : base(viewModel, "Synchronization")
	{
		InitializeComponent();
	}

}

