/*
 * Created by SharpDevelop.
 * User: ctc0065
 * Date: 2018/01/31
 * Time: 11:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace ProjectDiffMaker
{
	[TestFixture]
	public class TestArtifactsDiffer
	{
		[Test]
		public void TestMethod()
		{
			ArtifactsDiffer differ = new ArtifactsDiffer(@"C:\DesignHistory\ea-artifacts\cuvic_aswea_master\20171208\project.bdprj",
			                                             @"C:\DesignHistory\ea-artifacts\cuvic_aswea_20180118\20180125\project.bdprj");
			// differ.mergeAllArtifacts();
			
		}
	}
}
