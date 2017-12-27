/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2017/12/27
 * Time: 9:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ElementIndexer
{
	class Program
	{
		public static void Main(string[] args)
		{
//			Console.WriteLine("Hello World!");
//			
//			// TODO: Implement Functionality Here
//			
			DatabaseWriter writer = new DatabaseWriter();
			writer.write();
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}