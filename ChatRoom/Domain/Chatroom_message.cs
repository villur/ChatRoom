//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChatRoom.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class Chatroom_message
    {
        public int Chatroom_message_Id { get; set; }
        public int user_id { get; set; }
        public string text { get; set; }
        public System.DateTime sent { get; set; }
    
        public virtual User User { get; set; }
    }
}