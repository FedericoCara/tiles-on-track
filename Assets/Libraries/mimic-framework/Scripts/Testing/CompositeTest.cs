using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.Testing
{
	public class CompositeTest : Test	{
		
		[SerializeField]
		private bool runInitOnSubtests = false;

		[SerializeField]
		private float intervalBetweenSubtests = 0.5f;

		[SerializeField]
		protected List<Test> subtests;

		private void Awake(){
			subtests.ForEach (subtest => subtest.RunAtStart = false);
		}

		public override IEnumerator Run ()	{
			foreach(Test subtest in subtests){
				if (subtest.isActiveAndEnabled) {
					if (runInitOnSubtests) {
						yield return subtest.InitAndRun ();
					} else {
						yield return subtest.BeforeRun ();
						yield return subtest.Run ();
						yield return subtest.AfterRun ();
					}
					yield return new WaitForSeconds (intervalBetweenSubtests);
				}
			}
		}
	}
}

