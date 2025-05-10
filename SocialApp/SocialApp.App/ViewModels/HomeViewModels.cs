using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.ViewModels
{
    public partial class HomeViewModels:BaseViewModel
    {
        private readonly IPostApi _postApi;

        public HomeViewModels(IPostApi postApi) 
        {
           _postApi = postApi;
        }
        public ObservableCollection<PostDto> Posts { get; set; } = [];
        private int _startIndex = 0; 
        private const int PageSize = 10;

        [RelayCommand]
        private async Task FetchPostAsync ()
        {
            await MakeApiCall(async () =>
            {
                var posts = await _postApi.GetPostsAsync(_startIndex, PageSize);
                if (posts.Length > 0)
                {
                    if (_startIndex == 0 && Posts.Count > 0)
                    {
                        //this is pull to refresh case 
                        //resey the post observable collection 
                        Posts.Clear();

                    }
                    _startIndex = posts.Length;
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }

                }


            });
               
        }

    }
}
