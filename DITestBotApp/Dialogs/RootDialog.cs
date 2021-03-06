﻿using DITestBotApp.Factories;
using DITestBotApp.Forms;
using DITestBotApp.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace DITestBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private IGreetService GreetService { get; }
        private IDialogFactory DialogFactory { get; }

        public RootDialog(IDialogFactory dialogFactory, IGreetService greetService)
        {
            this.DialogFactory = dialogFactory;
            this.GreetService = greetService;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.GreetInteractionAsync);
            return Task.CompletedTask;
        }

        public async Task GreetInteractionAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(this.GreetService.GetMessage());
            await this.MainInteractionAsync(context, result);
        }

        public async Task MainInteractionAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Text == "change")
            {
                context.Call(this.DialogFactory.Create<ISimpleDialog>(), this.ReturnFromSimpleDialogInteractionAsync);
            }
            else if (activity.Text == "form")
            {
                context.Call(FormDialog.FromForm(Customer.BuildForm, FormOptions.PromptInStart), this.ReturnFromCustomerForm);
            }
            else
            {
                var length = (activity.Text ?? string.Empty).Length;
                await context.PostAsync($"You sent {activity.Text} which was {length} characters");
                context.Wait(this.MainInteractionAsync);
            }
        }

        public async Task ReturnFromCustomerForm(IDialogContext context, IAwaitable<Customer> result)
        {
            var customer = await result;
            await context.PostAsync($"{customer.Name} {customer.Age} {customer.Type}");
            context.Wait(this.MainInteractionAsync);
        }

        public async Task ReturnFromSimpleDialogInteractionAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("returned");
            context.Wait(this.MainInteractionAsync);
        }
    }
}