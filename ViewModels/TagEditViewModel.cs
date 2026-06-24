using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using System;

namespace RecipeManager.ViewModels;

public partial class TagEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    private readonly Tag _originalTag;
    private readonly Action _closeAction;

    public TagEditViewModel(Tag tag, Action closeAction)
    {
        _originalTag = tag;
        _closeAction = closeAction;
        name = tag.Name;
    }

    [RelayCommand]
    private void Save()
    {
        _originalTag.Name = name;
        _closeAction?.Invoke();
    }
}