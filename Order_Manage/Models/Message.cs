﻿using System.ComponentModel.DataAnnotations;

namespace Order_Manage.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? Content { get; set; }
    }
}
