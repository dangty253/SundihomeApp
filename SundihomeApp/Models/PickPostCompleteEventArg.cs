using System;
using SundihomeApi.Entities;

namespace SundihomeApp.Models
{
    public class PickPostCompleteEventArg : EventArgs
    {
        public Post PostSelected { get; set; }
    }
}
