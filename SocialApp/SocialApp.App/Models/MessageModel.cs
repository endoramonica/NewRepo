using CommunityToolkit.Mvvm.ComponentModel;
using SocialAppLibrary.Shared.Dtos;
using System;
using System.ComponentModel.DataAnnotations;

namespace SocialApp.App.Models
{
    /// <summary>
    /// Represents an individual chat message in the UI.
    /// </summary>
    public partial class MessageModel : ObservableObject
    {
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public int MessageType { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? ReadDateTime { get; set; }

        // Add constructor

        public static MessageModel FromLoggedInUser(MessageDto dto, Guid currentUserId)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new MessageModel
            {
                Id = dto.ToUserId,
                FromUserId = dto.FromUserId,    
                Content = dto.Content,
                SentAt = dto.SendDateTime,
                IsRead = dto.IsRead,
                


            };
        }

    }
}