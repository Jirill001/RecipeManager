using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using RecipeManager.Services;
using System.Collections.ObjectModel;

namespace RecipeManager.ViewModels;

public partial class TagListViewModel : ObservableObject
{
    private readonly TagService _tagService;

    public TagListViewModel(TagService tagService)
    {
        _tagService = tagService;
        LoadTags();
    }

    public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();

    [RelayCommand]
    private void AddTag()
    {
        var tag = new Tag();
        _tagService.Add(tag);
        Tags.Add(tag);
    }

    [RelayCommand]
    private void DeleteTag(Tag tag)
    {
        if (tag != null)
        {
            _tagService.Delete(tag.Id);
            Tags.Remove(tag);
        }
    }

    private void LoadTags()
    {
        Tags.Clear();
        foreach (var tag in _tagService.GetAll())
        {
            Tags.Add(tag);
        }
    }
}