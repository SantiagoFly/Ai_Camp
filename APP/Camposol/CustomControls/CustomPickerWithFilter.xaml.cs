using Camposol.Models;
using System.Windows.Input;

namespace Camposol.CustomControls;

public partial class CustomPickerWithFilter : ContentView
{
	public static BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomPickerWithFilter), propertyChanged: (bindable, oldValue, newValue) =>
		{
			var control = (CustomPickerWithFilter)bindable;
			control.TitleLabel.Text = newValue.ToString();
		});

    public static BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(List<Lote>), typeof(CustomPickerWithFilter), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (CustomPickerWithFilter)bindable;
        control.Collection.ItemsSource = (List<Lote>)newValue;
    });

    public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(Lote), typeof(CustomPickerWithFilter), defaultBindingMode: BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (CustomPickerWithFilter)bindable;
        control.Collection.SelectedItem = (Lote)newValue;
    });

    public static readonly BindableProperty OkSelectLoteCommandProperty = BindableProperty.Create(nameof(OkSelectLoteCommand), typeof(ICommand), typeof(CustomPickerWithFilter), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (CustomPickerWithFilter)bindable;
        control.btnAccept.Command = (ICommand)newValue;
    });

    public static readonly BindableProperty CancelSelectLoteCommandProperty = BindableProperty.Create(nameof(CancelSelectLoteCommand), typeof(ICommand), typeof(CustomPickerWithFilter), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (CustomPickerWithFilter)bindable;
        control.btnCancel.Command = (ICommand)newValue;
    });

    public CustomPickerWithFilter()
	{
		InitializeComponent();
	}

	public string Title 
	{
		get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
	}

    public List<Lote> Items
    {
        get => (List<Lote>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public Lote SelectedItem
    {
        get => (Lote)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public ICommand OkSelectLoteCommand
    {
        get => (ICommand)GetValue(OkSelectLoteCommandProperty);
        set => SetValue(OkSelectLoteCommandProperty, value);
    }
    
    public ICommand CancelSelectLoteCommand
    {
        get => (ICommand)GetValue(CancelSelectLoteCommandProperty);
        set => SetValue(CancelSelectLoteCommandProperty, value);
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue)) 
        {
            Collection.ItemsSource = Items;
        }
        else
        {
            Collection.ItemsSource = Items.Where(x => x.Name.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
        }
    }


    private void Collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItem = (Lote)Collection.SelectedItem;
    }
}