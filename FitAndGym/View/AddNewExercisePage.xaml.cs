﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FitAndGym.Infrastructure;
using FitAndGym.Models;
using FitAndGym.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FitAndGym.View
{
    public partial class AddNewExercisePage : PhoneApplicationPage
    {
        public AddNewExercisePage()
        {
            BuildLocalizedApplicationBar();
            InitializeComponent();

            // fill ListPicker
            NewExIntensityListPicker.ItemsSource = Enum.GetValues(typeof(Intensity));
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var saveChangesButton = new ApplicationBarIconButton(new Uri("/Images/save.png", UriKind.RelativeOrAbsolute));
            saveChangesButton.Click += saveChanges_Click;
            saveChangesButton.Text = AppResources.SaveChangesAppBar;
            ApplicationBar.Buttons.Add(saveChangesButton);

            var discardChangesButton = new ApplicationBarIconButton(new Uri("/Images/cancel.png", UriKind.RelativeOrAbsolute));
            discardChangesButton.Click += discardChangesButton_Click;
            discardChangesButton.Text = AppResources.DiscardChangesAppBar;
            ApplicationBar.Buttons.Add(discardChangesButton);
        }

        private void discardChangesButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void saveChanges_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}