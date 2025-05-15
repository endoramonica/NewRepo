using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SocialApp.App.Models;

namespace SocialApp.App.Templates
{
    public class PostTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WithImage { get; set; }
        public DataTemplate WithNoImage { get; set; }
        public DataTemplate OnlyImage { get; set; }

      
        protected override Microsoft.Maui.Controls.DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
           if( item is PostModel post )
            {
                // only text content 
                if(string.IsNullOrWhiteSpace(post.PhotoUrl) )
                {
                    return WithNoImage;
                }
                // only Photo Image
                if(string.IsNullOrWhiteSpace(post.Content) )
                {
                    return OnlyImage;
                }
                // both content and image 
                return WithImage;
            }
            return null;
        }
    }
}
