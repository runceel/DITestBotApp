using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DITestBotApp.Forms
{
    public enum CustomerType
    {
        None,
        Normal,
        Premium,
    }

    [Serializable]
    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public CustomerType Type { get; set; }

        public static IForm<Customer> BuildForm()
        {
            return new FormBuilder<Customer>()
                .Message("カスタマーの情報を入れてね")
                .Build();
        }
    }
}