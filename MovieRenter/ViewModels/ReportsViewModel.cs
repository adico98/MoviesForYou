using ClosedXML.Excel;
using Microsoft.Win32;
using MovieRenter.Models;
using MovieRenter.Command;
using MovieRenter.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private ReportsEnum seletcedReport;
        private List<MovieModel> allMoviesData;

        // check what report was selected
        public ReportsEnum SeletcedReport
        {
            get => seletcedReport;
            set
            {
                seletcedReport = value;
                OnPropertyChanged("SelectedReport");
            }
        }

        // load the ReportsEnum Values
        public IEnumerable<ReportsEnum> ReportsEnumValues
        {
            get
            {
                return Enum.GetValues(typeof(ReportsEnum))
                    .Cast<ReportsEnum>();
            }
        }

        public ReportsViewModel()
        {
            allMoviesData = DBOperations.GetAllMoviesData().Result;
        }

        // Get how many users there are in the system 
        public int UsersCount
        {
            get => DBOperations.GetAllUsersData().Result.Count;
        }

        // Get how many movies the system has 
        public int MoviesCount
        {
            get => allMoviesData.Count;
        }

        // Get how many genres the system has 
        public int GenresCount
        {
            get => DBOperations.GetAllGenresDataCount();
        }

        // Get how many approve reviews the system has 
        public int ReviewsCount
        {
            get => DBOperations.GetAllReviewsDataCount();
        }

        // Get how much movies rented 
        public int RentedMvoiesCount
        {
            get => DBOperations.GetAllRentedMoviesDataCount();
        }

        // Get how much moview are currently being rented
        public int CurrRentedMoviesCount
        {
            get => DBOperations.GetAllCurrRentedMoviesDataCount();
        }

        private ICommand createReportCommand;
        public ICommand CreateReportCommand
        {
            get
            {
                return createReportCommand ?? (createReportCommand = new CommandHandler(() => CreateReport(), true));
            }
        }

        private void CreateReport()
        {
            // check that a report was chosen 
            if (SeletcedReport != ReportsEnum.SelectReport)
            {
                try
                {
                    //open a dialog to get the report saving location 
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx | New Excel file (*.xlsm) | *.xlsm";
                    saveFileDialog.FileName = SeletcedReport.ToString();
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Get the saving location the user chose
                        string fileName = saveFileDialog.FileName;

                        // Open an excel file and open a new sheet named as the display name in the enum value that was chosen 
                        var wb = new XLWorkbook();
                        var ws = wb.Worksheets.Add(SeletcedReport.GetAttributeOfType<DisplayAttribute>().Name);

                        // check what report was chosen and update the excel to have the colunm header and all the data for that report 
                        switch (SeletcedReport)
                        {
                            case ReportsEnum.MostRentedMovies:

                                ws.Cell(1, 1).Value = "Movie Id";
                                ws.Cell(1, 2).Value = "Movie Title";
                                ws.Cell(1, 3).Value = "Renting Amount";
                                ws.Cell(2, 1).InsertData(DBOperations.GetMostRentedMovies().Result);
                                break;

                            case ReportsEnum.MostRentedGenres:
                                ws.Cell(1, 1).Value = "Genre Id";
                                ws.Cell(1, 2).Value = "Genre Name";
                                ws.Cell(1, 3).Value = "Renting Amount";
                                ws.Cell(2, 1).InsertData(DBOperations.GetMostRentedGenres().Result);
                                break;

                            case ReportsEnum.MostReviewedMovies:

                                ws.Cell(1, 1).Value = "Movie Id";
                                ws.Cell(1, 2).Value = "Movie Title";
                                ws.Cell(1, 3).Value = "Rating";
                                ws.Cell(1, 4).Value = "Reviews Number";
                                List<MovieModel> moviesList = allMoviesData.OrderByDescending(o => o.ReviewCount).ToList();
                                var reducedMoviesList = moviesList.Select(e => new { e.MovieId, e.Title, e.Rating, e.ReviewCount }).ToList();
                                ws.Cell(2, 1).InsertData(reducedMoviesList);
                                break;

                            case ReportsEnum.TopRatedMovies:

                                ws.Cell(1, 1).Value = "Movie Id";
                                ws.Cell(1, 2).Value = "Movie Title";
                                ws.Cell(1, 3).Value = "Reviews Number";
                                ws.Cell(1, 4).Value = "Rating";
                                List<MovieModel> topMoviesList = allMoviesData.OrderByDescending(o => o.Rating).ToList();
                                var reducedTopMoviesList = topMoviesList.Select(e => new { e.MovieId, e.Title, e.ReviewCount, e.Rating }).ToList();
                                ws.Cell(2, 1).InsertData(reducedTopMoviesList);
                                break;
                            case ReportsEnum.UsersList:
                                List<UserModel> usersList = DBOperations.GetAllUsersData().Result;
                                PropertyInfo[] properties = usersList.First().GetType().GetProperties();
                                List<string> headerNames = properties.Select(prop => prop.Name).ToList();
                                for (int i = 0; i < headerNames.Count; i++)
                                {
                                    ws.Cell(1, i + 1).Value = headerNames[i];
                                }
                                ws.Cell(2, 1).InsertData(usersList);
                                break;
                            case ReportsEnum.GenresList:
                                List<GenreModel> genreList = DBOperations.GetAllGenresData().Result;
                                PropertyInfo[] prop = genreList.First().GetType().GetProperties();
                                List<string> header = prop.Select(prop => prop.Name).ToList();
                                for (int i = 0; i < header.Count; i++)
                                {
                                    ws.Cell(1, i + 1).Value = header[i];
                                }
                                ws.Cell(2, 1).InsertData(genreList);
                                break;


                        }


                                // save the excel file on the compueter
                                wb.SaveAs(fileName);
                        
                    }
                } catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public enum ReportsEnum
    {
        [Display(Name = "Select Report")]
        SelectReport,
        [Display(Name = "Most Rented Movies")]
        MostRentedMovies,
        [Display(Name = "Most Reviewed Movies")]
        MostReviewedMovies,
        [Display(Name = "Top Rated Movies")]
        TopRatedMovies,
        [Display(Name = "Most Rented Genres")]
        MostRentedGenres,
        [Display(Name = "Users List")]
        UsersList,
        [Display(Name = "Genres List")]
        GenresList

    }
}
