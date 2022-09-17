using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnitTestBoilerplate.Model;

namespace UnitTestBoilerplate.View
{
	/// <summary>
	/// Interaction logic for TestFileConflict.xaml
	/// </summary>
	public partial class TestFileConflict : BaseDialogWindow
	{
		public TestFileConflict(string fileName)
		{
			this.InitializeComponent();
			this.messageTextBlock.Text = "Test file already exists: " + fileName;
		}

		public TestFileConflictResult Result { get; set; } = TestFileConflictResult.Cancel;

		private void OnOverwriteClick(object sender, RoutedEventArgs e)
		{
			this.Result = TestFileConflictResult.Overwrite;
			this.Close();
		}

		private void OnNewNameClick(object sender, RoutedEventArgs e)
		{
			this.Result = TestFileConflictResult.UseNewName;
			this.Close();
		}

		private void OnCancelClick(object sender, RoutedEventArgs e)
		{
			this.Result = TestFileConflictResult.Cancel;
			this.Close();
		}
	}
}
