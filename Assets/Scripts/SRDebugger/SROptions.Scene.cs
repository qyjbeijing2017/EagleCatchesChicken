using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using SRDebugger;
using UnityEngine;

public partial class SROptions
{
		[Category("Open Practice Scene")] // Options will be grouped by category
	public void OpenPracticeScene() {
		GameManager.instance.LoadScene("Practice");
	}
}
