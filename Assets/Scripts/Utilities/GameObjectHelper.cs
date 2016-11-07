/// <summary>
/// Game object helper.
/// 
/// Static helper functions for common GameObject tasks.
/// 
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public class GameObjectHelper 
{
	public static GameObject FindChild(GameObject gameObject, string childName, bool clearSubChildren)
	{
		GameObject child = null;
		
		for(int i = 0; i < gameObject.transform.childCount && child == null; ++i)
		{
			Transform currentChild = gameObject.transform.GetChild(i);
			
			if(currentChild.gameObject.name == childName)
			{
				child = currentChild.gameObject;
			}
		}
		
		if(child == null)
		{
			child = new GameObject(childName);
			child.transform.parent = gameObject.transform;
		}
		else if(clearSubChildren)
		{
			while(child.transform.childCount > 0)
			{
				GameObject.DestroyImmediate(child.transform.GetChild(0).gameObject);
			}
		}
		
		return child;
	}
	
	// Searches up :{
	public static GameObject SearchForComponent(GameObject searchFocus, System.Type targetType)
	{
		GameObject current = searchFocus.transform.parent.gameObject;
		
		while(current != null)
		{
			// Search breadth then go up.
			int childCount = current.transform.childCount;
			Component searchComponent = null;
			
			for(int i = 0; i < childCount; ++i)
			{
				GameObject child = current.transform.GetChild(i).gameObject;
				
				if(child == searchFocus)
				{
					continue;	
				}
				
				searchComponent = child.GetComponent(targetType);
				if(searchComponent != null)
				{
					return searchComponent.gameObject;		
				}
			}
			
			// Search the node itself. This is needed for the root node. TODO: Only do it then, duh.
			searchComponent = current.gameObject.GetComponent(targetType);
			if(searchComponent != null)
			{
				return searchComponent.gameObject;		
			}
			
			current = current.transform.parent.gameObject;	
		}
		
		return null;
	}

	public static T SearchParentsForComponent<T>(GameObject searchFocus) where T : Component
	{
		Transform current = searchFocus.transform;
		T foundObject = null;

		while (current != null)
		{
			foundObject = current.GetComponent<T>() as T;
			if (foundObject != null)
			{
				return foundObject;
			}
			else
			{
				current = current.transform.parent;
			}
		}

		return null;
	}
	
	// Recursive bullshit
	public static List<GameObject> FindAllChildrenWithTag(GameObject gameObject, string tag)
	{
		List<GameObject> foundObjects = new List<GameObject>();
			
		for(int i = 0; i < gameObject.transform.childCount; ++i)
		{
			Transform currentChild = gameObject.transform.GetChild(i);
			
			if(currentChild.gameObject.tag == tag)
			{
				foundObjects.Add(currentChild.gameObject);
			}
			
			foundObjects.AddRange(FindAllChildrenWithTag(currentChild.gameObject, tag));
		}
		
		return foundObjects;
	}
	
	// Recursive bullshit
	public static List<GameObject> FindAllChildrenWithLayer(GameObject gameObject, LayerMask layer)
	{
		List<GameObject> foundObjects = new List<GameObject>();
			
		for(int i = 0; i < gameObject.transform.childCount; ++i)
		{
			Transform currentChild = gameObject.transform.GetChild(i);
			
			if(currentChild.gameObject.layer == layer)
			{
				foundObjects.Add(currentChild.gameObject);
			}
			
			foundObjects.AddRange(FindAllChildrenWithLayer(currentChild.gameObject, layer));
		}
		
		return foundObjects;
	}
	
	public static void LogQuaternionEuler(Quaternion quaternion)
	{
		Debug.Log("Quaternion: " + quaternion.eulerAngles.x + ", " + quaternion.eulerAngles.y + ", " + quaternion.eulerAngles.z);
	}

	public static string LogHierarchy(GameObject gameObject)
	{
		List<string> strings = new List<string>();

		while(true)
		{
			strings.Add(gameObject.name);
			if (gameObject.transform.parent != null)
			{
				gameObject = gameObject.transform.parent.gameObject;
			}
			else
			{
				break;
			}
			
		}

		string output = string.Empty;

		int indent = 0;
		foreach(var name in strings)
		{
			if(indent > 0)
			{
				output += "\n";
			}

			for(int i = 0; i < indent; i++)
			{
				output += "\t";
			}

			output += name;

			indent++;
		}

		return output;
	}

	public static List<T> GetAllChildren<T>(GameObject targetObject) where T : Component
	{
		List<T> returnList = new List<T>();

		for (int i = 0; i < targetObject.transform.childCount; i++ )
		{
			T currentObject = targetObject.transform.GetChild(i).GetComponent<T>() as T;
			if (currentObject != null)
			{
				returnList.Add(currentObject);
				
			}
			returnList.AddRange(GetAllChildren<T>(targetObject.transform.GetChild(i).gameObject));

		}

		return returnList;
	}

    public static List<Component> GetAllChildren(GameObject targetObject, System.Type componentType) 
    {
        List<Component> returnList = new List<Component>();

        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            Component currentComponent = targetObject.transform.GetChild(i).GetComponent(componentType) ;
            if (currentComponent != null)
            {
                returnList.Add(currentComponent);

            }
            returnList.AddRange(GetAllChildren(targetObject.transform.GetChild(i).gameObject, componentType));

        }

        return returnList;
    }

    public static List<Component> GetAllEnabledChildren(GameObject targetObject, System.Type componentType)
    {
        List<Component> returnList = new List<Component>();

        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            Component currentComponent = targetObject.transform.GetChild(i).GetComponent(componentType);
            if (currentComponent != null)
            {
                returnList.Add(currentComponent);

            }
            returnList.AddRange(GetAllChildren(targetObject.transform.GetChild(i).gameObject, componentType));

        }

        return returnList;
    }

	public static int GetHierarchyDepth(GameObject targetObject)
	{
		int depth = 0;

		while (targetObject.transform.parent != null)
		{
			targetObject = targetObject.transform.parent.gameObject;
			depth++;
		}
		return depth;
	}

    // This is a really grimey way to get a 
    public int GetObjectHash(GameObject targetObject)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        while (true)
        {
            builder.Append(".");
            builder.Append(targetObject.name);
            if (targetObject.transform.parent != null)
            {
                targetObject = targetObject.transform.parent.gameObject;
            }
            else
            {
                break;
            }

        }

        return builder.ToString().GetHashCode();
    }
}
