using System;
namespace SimpleEchoBot.Dialogs
{
    public class UserInfo
    {
        private String name;
        private String email;
        private String location;

        public UserInfo()
        {
            name = null;
            email = null;
            location = null;
        }

        public UserInfo SetName(String name) {
            this.name = name;
            return this;
        }

        public UserInfo SetEmail(String email) {
            this.email = email;
            return this;
        }

        public UserInfo SetLocation(String location) {
            this.location = location;
            return this;
        }

        public String GetEmail() {
            return this.email;
        }

        public String GetName() {
            return this.name;
        }

        public String GetLocation() {
            return this.location;
        }
    }
}
