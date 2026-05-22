using UnityEngine;

[CreateAssetMenu(fileName = "Category_", menuName = "Quest/Task/Category")]
public class Category : ScriptableObject
{
    [SerializeField]
    private string categoryUID;

    public string CategoryUID => categoryUID;

    public bool Equals(Category category)
    {
        if(category == null) 
            return false;

        if(GetType() != category.GetType()) 
            return false;

        if(ReferenceEquals(category, this)) 
            return true;

        return category.categoryUID == categoryUID;
    }
}
