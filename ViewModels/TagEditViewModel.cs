using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;

namespace RecipeManager.ViewModels;

public partial class TagEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    private readonly Tag _originalTag;

    public TagEditViewModel(Tag tag)
    {
        _originalTag = tag;
        name = tag.Name;
    }

    [RelayCommand]
    private void Save()
    {
        _originalTag.Name = name;
    }
}