// Copyright (c) 2010 Bob Berkebile
// Please direct any bugs/comments/suggestions to http://www.pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Collections;

namespace Utility
{
	[CustomEditor(typeof(PathUtility))]
	public class PathUtilityEditor : Editor
	{
		PathUtility _target;
		GUIStyle style = new GUIStyle ();
		public static int count = 0;
		protected int offset = 1;
	
		void OnEnable ()
		{
			//i like bold handle labels since I'm getting old:
			style.fontStyle = FontStyle.Bold;
			style.normal.textColor = Color.white;
			_target = (PathUtility)target;
		
			//lock in a default path name:
			if (!_target.initialized) {
				_target.initialized = true;
				_target.pathName = "New Path " + ++count;
				_target.initialName = _target.pathName;
			}
		}
	
		public override void OnInspectorGUI ()
		{		
			//draw the path?
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PrefixLabel ("Path Visible");
			_target.pathVisible = EditorGUILayout.Toggle (_target.pathVisible);
			EditorGUILayout.EndHorizontal ();
		
			//path name:
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PrefixLabel ("Path Name");
			_target.pathName = EditorGUILayout.TextField (_target.pathName);
			EditorGUILayout.EndHorizontal ();
		
			if (_target.pathName == "") {
				_target.pathName = _target.initialName;
			}
		
			//path color:
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PrefixLabel ("Path Color");
			_target.pathColor = EditorGUILayout.ColorField (_target.pathColor);
			EditorGUILayout.EndHorizontal ();
		
			//exploration segment count control:
			EditorGUILayout.BeginHorizontal ();
			//EditorGUILayout.PrefixLabel("Node Count");
			_target.nodeCount = Mathf.Max (2, EditorGUILayout.IntField ("Node Count", _target.nodeCount));
			//_target.nodeCount =  Mathf.Clamp(EditorGUILayout.IntSlider(_target.nodeCount, 0, 10), 2,100);
			EditorGUILayout.EndHorizontal ();
		
			//add node?
			if (_target.nodeCount > _target.nodes.Count) {
				for (int i = 0; i < _target.nodeCount - _target.nodes.Count; i++) {
					_target.nodes.Add (Vector3.zero);	
				}
			}
	
			//remove node?
			if (_target.nodeCount < _target.nodes.Count) {
				if (EditorUtility.DisplayDialog ("Remove path node?", "Shortening the node list will permantently destory parts of your path. This operation cannot be undone.", "OK", "Cancel")) {
					int removeCount = _target.nodes.Count - _target.nodeCount;
					_target.nodes.RemoveRange (_target.nodes.Count - removeCount, removeCount);
				} else {
					_target.nodeCount = _target.nodes.Count;	
				}

			}
				
			//node display:
			EditorGUI.indentLevel = 4;
			for (int i = 0; i < _target.nodes.Count; i++) {
				_target.nodes [i] = EditorGUILayout.Vector3Field ("Node " + (i + 1), _target.nodes [i]);
			}
			if (GUILayout.Button ("Add Node")) {
				int count = _target.nodes.Count;
				if (count >= 2)
					_target.nodes.Add (_target.nodes [count - 1]+(_target.nodes [count - 1] - _target.nodes [count - 2]));
				else
					_target.nodes.Add (Vector3.zero);
				_target.nodeCount++;


				//Array concatenation
				//				Vector3[] array1 = Utility_Path.GetPathNodesLocal("Circulatory_Path_AorticArch_Back");
				//				Vector3[] array2 =  _target.nodes.ToArray();
				//				Vector3[] newArray = array1.Concat(array2).ToArray();
				//				_target.nodes=newArray.OfType<Vector3>().ToList();
			}

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("Offset")) {
				_target.Offset(offset);
			}
			offset = EditorGUILayout.IntField(offset);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			if (GUILayout.Button ("Reverse")) {
				_target.Reverse();
			}
		
			//update and redraw:
			if (GUI.changed) {
				EditorUtility.SetDirty (_target);			
			}
		}
	
		void OnSceneGUI ()
		{
			if (_target.pathVisible) {	
			
				Vector3[] nodes = _target.Nodes;

				if (nodes.Length > 0) {
					//allow path adjustment undo:
					Undo.RecordObject (_target, "Adjust iTween Path");

					//node handle display:
					for (int i = 0; i < nodes.Length; i++) {
						Handles.Label (nodes [i], (i + 1) + "/" + (nodes.Length), style);
						if(Tools.pivotRotation==PivotRotation.Global||i==0){
							_target.nodes [i] = Handles.PositionHandle (nodes [i], Quaternion.identity) - _target.transform.position;
						}else{
							_target.nodes [i] = Handles.PositionHandle (nodes [i], Quaternion.LookRotation(nodes[i]-nodes[i-1])) - _target.transform.position;
						}
					}	
				}	
			}
		}
	}
}