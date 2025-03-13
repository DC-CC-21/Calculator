using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MathEvaluation;
using MathEvaluation.Context;

namespace Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string Equation = "";
    private Graph graph;
    private FileOperations fileOperations = new();

    public MainWindow()
    {
        InitializeComponent();
        graph = new(Canvas, Output, State);
        AddEvent();
        FetchPreviousEqs();
    }

    /// <summary>
    /// This function builds the Grid, Equation Button, and X Button for the sidebar
    /// </summary>
    /// <param name="value"></param>
    private void CreatePreviousEqButton(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        // *,25 column grid to hold the equation and x button for the sidebar
        Grid grid = new();
        grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new() { Width = new(25, GridUnitType.Pixel) });
        Previous.Children.Add(grid);


        // Equation button
        Button btn = new()
        {
            Content = value,
            Style = grid.FindResource("PrevBtn") as Style
        };
        btn.Click += ClickEquation;


        // X button
        Button xBtn = new()
        {
            Content = "X",
            Style = grid.FindResource("XBtn") as Style
        };

        xBtn.Click += DeletePreviousEq;
        Grid.SetColumn(xBtn, 1);

        // Add both the Equation and X button to the grid
        grid.Children.Add(btn);
        grid.Children.Add(xBtn);


        // Scroll the scrollViewer to the bottom so the user can see the most current entry
        ScrollViewer? scrollViewer = Previous.Parent as ScrollViewer;
        scrollViewer?.ScrollToBottom();


    }

    /// <summary>
    /// This function is used to add a new equation to both the sidebar and 
    /// the output txt file
    /// </summary>
    private void AddPreviousEq()
    {
        CreatePreviousEqButton(Equation);
        fileOperations.WriteFile(Equation);

        // If the Previous equation count is greater than 15, remove the oldest
        // entry from the sidebar and the output list
        if (Previous.Children.Count > 15)
        {
            Previous.Children.RemoveAt(0);
            fileOperations.DeleteEq();
        }

    }

    /// <summary>
    /// This function calls the file operation to read from the equation output file
    /// it then loops through the list of previous equations and adds them to the 
    /// previous button sidebar
    /// </summary>
    private void FetchPreviousEqs()
    {
        List<string> data = fileOperations.ReadFile();

        foreach (string value in data)
        {
            CreatePreviousEqButton(value);
        }
    }

    /// <summary>
    /// This function is used to delete a specific equation when the "X" button is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeletePreviousEq(object sender, RoutedEventArgs e)
    {
        Button btn = (sender as Button)!;
        string content = btn.Content.ToString() ?? "";

        Previous.Children.Remove(btn.Parent as UIElement);
        fileOperations.DeleteEq(content);

    }

    /// <summary>
    /// Function that is called when a previous entry button is clicked
    /// 
    /// This function collects the previous function from the clicked button's
    /// Text and then appends it to the current equation string
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClickEquation(object sender, RoutedEventArgs e)
    {
        Button btn = (sender as Button)!;
        Equation += btn.Content.ToString() ?? "";
        SetOutput();
    }

    /// <summary>
    /// Adds the click event to all buttons in the grid
    /// </summary>
    private void AddEvent()
    {
        foreach (var child in ButtonGrid.Children)
        {
            if (child is Button)
            {
                Button btn = (child as Button)!;
                if (btn.Tag is null)
                {
                    btn.Click += Button_Click;
                }
            }
        }

    }

    /// <summary>
    /// Handles the click event of the buttons
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        object? senderContent = (sender as Button)?.Content;
        // Apply the button content to the equation if it is not null
        if (senderContent is not null)
        {
            Equation += senderContent.ToString();
            SetOutput();
        }
    }

    /// <summary>
    /// Handles the click event of the delete button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Delete(object sender, RoutedEventArgs e)
    {
        // Delete the last character in the equation
        if (Equation.Length > 0)
        {
            graph.Stop();
            Equation = Equation[..^1];
        }
        SetOutput();
    }

    /// <summary>
    /// Handles the click event of the clear button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Clear(object sender, RoutedEventArgs e)
    {
        // Clear the equation, stop the graph, and clear the canvas
        graph.Stop();
        Equation = "";
        Canvas.Children.Clear();
        SetOutput();
    }


    /// <summary>
    /// Handles the click event of the enter button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Enter(object sender, RoutedEventArgs e)
    {
        // Check if the equation is empty
        if (Equation != "")
        {
            Debug.WriteLine("Clicked");
            AddPreviousEq();
            // If the equation contains an x, start the graph else evaluate the equation
            if (Equation.Contains('x'))
            {
                graph.StartGraph(Equation);
            }
            else
            {
                double? result = Math.EvaluateMath(Equation, Output);
                if (result is not null)
                {
                    Equation = result!.ToString()!;
                    SetOutput();
                }
            }
        }
    }

    /// <summary>
    /// Sets the output of the calculator
    /// </summary>
    private void SetOutput()
    {
        Output.Content = Equation;
    }

    /// <summary>
    /// Handles the DispatcherUnhandledException event of the App control.
    /// This function is used to prevent the application from crashing when an exception is thrown.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {

        e.Handled = true;
    }
}