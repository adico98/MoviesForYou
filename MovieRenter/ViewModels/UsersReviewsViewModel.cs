using MailKit.Net.Smtp;
using MimeKit;
using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualBasic;
using MovieRenter.Views;

namespace MovieRenter.ViewModels
{
    public class UsersReviewsViewModel : ViewModelBase
    {

        public UsersReviewsViewModel()
        {
            PendingAprrovalReviews = new ObservableCollection<ReviewModel>();
            ApproveReviews = new ObservableCollection<ReviewModel>();

            // Load Pending and Approve Reviews 
            PendingAprrovalReviews.Clear();
            foreach (var review in DBOperations.GetAllReviewsData(true).Result)
            {
                PendingAprrovalReviews.Add(review);
            }

            ApproveReviews.Clear();
            foreach (var review in DBOperations.GetAllReviewsData(false).Result)
            {
                ApproveReviews.Add(review);
            }
        }

        public ObservableCollection<ReviewModel> PendingAprrovalReviews { get; set; }
        public ObservableCollection<ReviewModel> ApproveReviews { get; set; }

        private ICommand approveReviewCommand;
        public ICommand ApproveReviewCommand
        {
            get
            {
                return approveReviewCommand ?? (approveReviewCommand = new CommandHandler(param => ApproveReview(param), true));
            }
        }

        private ICommand declineReviewCommand;
        public ICommand DeclineReviewCommand
        {
            get
            {
                return declineReviewCommand ?? (declineReviewCommand = new CommandHandler(param => DeclineReview(param), true));
            }
        }

        private ICommand deleteReviewCommand;
        public ICommand DeleteReviewCommand
        {
            get
            {
                return deleteReviewCommand ?? (deleteReviewCommand = new CommandHandler(param => DeleteReview(param), true));
            }
        }

        // Approve a review and send email to the email of the user to let them know the review was approved 
        private void ApproveReview(object param)
        {
            if (DBOperations.ApproveReview((ReviewModel)param))
            {
                PendingAprrovalReviews.Remove(((ReviewModel)param));
                ApproveReviews.Add(((ReviewModel)param));
                SendUpdatedReviewEmail((ReviewModel)param, ReviewUpdate.Approve, "");
            }

        }

        // if the review was decline, get a reason from the admin to why the review was decline
        // Send email to the user to let them know that the review was decline and delete it from the database 
        private void DeclineReview(object param)
        {
            var reason = GetReason();
            if (reason == string.Empty)
            {
                MessageBox.Show("You need to enter the reason for declining this review", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DBOperations.DeleteReview((ReviewModel)param))
            {
                // update
                PendingAprrovalReviews.Remove(((ReviewModel)param));
                SendUpdatedReviewEmail((ReviewModel)param, ReviewUpdate.Decline, reason);
            }

        }

        // Delete a review from the approved review, add a reason to why it was delete and send an updated email to the user 
        private void DeleteReview(object param)
        {
            var reason = GetReason();
            if (reason == string.Empty)
            {
                MessageBox.Show("You need to enter the reason for deleting this review", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DBOperations.DeleteReview((ReviewModel)param))
            {
                // update

                ApproveReviews.Remove(((ReviewModel)param));
                SendUpdatedReviewEmail((ReviewModel)param, ReviewUpdate.Deleted, reason);
            }
        }

        // open a dialog and ask the admin for a reason why he decline / delete a review for the user 
        private string GetReason()
        {
            DeleteReviewReason addReason = new DeleteReviewReason();
            if ((bool)addReason.ShowDialog())
            {
                return addReason.Answer;
            }
            return string.Empty;
        }

        // Send an email to the user to update him on his review status 
        private void SendUpdatedReviewEmail(ReviewModel currReview, ReviewUpdate reviewUpdate, string reason)
        {
            // Get the reviewer user details 
            UserModel reviewerUser = DBOperations.GetAllUsersData().Result.Find(x => x.Username == currReview.Username);
            string fullname = reviewerUser.Firstname + " " + reviewerUser.LastName;

            // check if the reviewer email is gmail, if not do not send an email
            if (!reviewerUser.Email.ToLower().Contains("gmail"))
                return;

            var message = new MimeMessage();
            
            // add the From and To in the email, and the subject of the email
            message.From.Add(new MailboxAddress("Review Update", Properties.Settings.Default.emailUserString));
            message.To.Add(new MailboxAddress(fullname, reviewerUser.Email ));
            message.Subject = "Review Update";

            string bodyText = "";


            // write the email message accordingly to the review update 
            if (reviewUpdate == ReviewUpdate.Approve)
            {

                bodyText = String.Format(@"Hi {0},

Your Review of {1} was approve!
You can see the review in the movie page on the app!

MovieRental", fullname, currReview.MovieTitle);

            } else if (reviewUpdate == ReviewUpdate.Decline)
            {
                bodyText = String.Format(@"Hi {0},

Your Review of {1} was unfortantly decline :(
The admin did not approve of the review beacause {2}.

MovieRental", fullname, currReview.MovieTitle, reason);

              
            } else if (reviewUpdate == ReviewUpdate.Deleted)
            {
                bodyText = String.Format(@"Hi {0},

Your Review of {1} was unfortantly delete :(
The admin deleted your review beacause {2}.

MovieRental", fullname, currReview.MovieTitle, reason);
            }

            message.Body = new TextPart("plain")
            {
                Text = bodyText
            };

            // Log in to the email address of the system and send the mail 
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Properties.Settings.Default.emailUserString, Properties.Settings.Default.emailConnString);
                  //  "movierentalreviewupdate@gmail.com", "clzgcduvjcsgtaxi");

                client.Send(message);
                client.Disconnect(true);
            }

        }
    }

    public enum ReviewUpdate
    {
        Approve, 
        Decline, 
        Deleted
    }
}
