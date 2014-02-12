
/* 
    See http://geekswithblogs.net/ranganh/archive/2006/03/25/73300.aspx#351705 for original source.
    Adapted to include "parent checked behavior".
*/

function AspNetTreeView_ApplyCascadeCheckmarks(treeViewID, args) {
    /*
        args param contents:
            - Index 0 is parent checked behavior:
                - "ALL" means "parent is checked if ALL children are checked".
                - "ANY" means "parent is checked if ANY child is checked".
                - "ANY_NO_UNTICK" means same as "ANY" but "parent is not unticked if all children are unticked".
                - Otherwise "checking/unchecking a child has not effect on the parent(s)".
    */

    var tv = GetElement(treeViewID);
    var checkBoxes = tv.getElementsByTagName("input");
        
    for (var i = 0; i < checkBoxes.length; i++)
    {
        if (checkBoxes[i].type == "checkbox")
        {
            $addHandler(checkBoxes[i], "click", function go(evt) { AspNetTreeView_CascadeCheckboxClick(evt, args); });
        }
    }
}

function AspNetTreeView_CascadeCheckboxClick(evt, args)
{
    var src = window.event != window.undefined ? window.event.srcElement : evt.target;
    var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");
    if(isChkBoxClick)
    {
        var parentTable = AspNetTreeView_GetParentByTagName("table", src);
        var nxtSibling = parentTable.nextSibling;
        if(nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node
        {
            if(nxtSibling.tagName.toLowerCase() == "div") //if node has children
            {
                //check or uncheck children at all levels
                AspNetTreeView_CheckUncheckChildren(parentTable.nextSibling, src.checked, args);
            }
        }
        //check or uncheck parents at all levels
        AspNetTreeView_CheckUncheckParents(src, src.checked, args);
    }
} 

function AspNetTreeView_CheckUncheckChildren(childContainer, check, args)
{
    var childChkBoxes = childContainer.getElementsByTagName("input");
    var childChkBoxCount = childChkBoxes.length;
    for(var i = 0; i<childChkBoxCount; i++)
    {
        childChkBoxes[i].checked = check;
    }
}

function AspNetTreeView_CheckUncheckParents(srcChild, check, args)
{
    if(args[0]) {    
        var parentDiv = AspNetTreeView_GetParentByTagName("div", srcChild);
        var parentNodeTable = parentDiv.previousSibling;

        if(parentNodeTable)
        {
            var checkUncheckSwitch;

            switch(args[0]) {
                case "ALL":
                    if(check) //checkbox checked
                    {
                        var isAllSiblingsChecked = AspNetTreeView_AreAllSiblingsChecked(srcChild);
                        if(isAllSiblingsChecked)
                            checkUncheckSwitch = true;
                        else 
                            return; //do not need to check parent if any(one or more) child not checked
                    }
                    else //checkbox unchecked
                    {
                        checkUncheckSwitch = false;
                    }
                    break;
                  
                case "ANY":
                    var isAnySiblingsChecked = AspNetTreeView_AreAnySiblingsChecked(srcChild);
                    if(isAnySiblingsChecked)
                        checkUncheckSwitch = true;
                    break;
                    
                case "ANY_NO_UNTICK":
                    checkUncheckSwitch = true;
                    break;
            }

            var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
            if(inpElemsInParentTable.length > 0)
            {
                var parentNodeChkBox = inpElemsInParentTable[0]; 
                parentNodeChkBox.checked = checkUncheckSwitch; 
                //do the same recursively
                AspNetTreeView_CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch, args);
            }
        }
    }
}

function AspNetTreeView_AreAllSiblingsChecked(chkBox)
{
    var parentDiv = AspNetTreeView_GetParentByTagName("div", chkBox);
    var childCount = parentDiv.childNodes.length;
    for(var i=0; i<childCount; i++)
    {
        if(parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node
        {
            if(parentDiv.childNodes[i].tagName.toLowerCase() == "table")
            {
                var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                //if any of sibling nodes are not checked, return false
                if(!prevChkBox.checked) 
                {
                    return false;
                } 
            }
        }
    }
    return true;
}

function AspNetTreeView_AreAnySiblingsChecked(chkBox)
{
    var parentDiv = AspNetTreeView_GetParentByTagName("div", chkBox);
    var childCount = parentDiv.childNodes.length;
    for(var i=0; i<childCount; i++)
    {
        if(parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node
        {
            if(parentDiv.childNodes[i].tagName.toLowerCase() == "table")
            {
                var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                //if any of sibling nodes are checked, return true
                if(prevChkBox.checked) 
                {
                    return true;
                } 
            }
        }
    }
    return false;
}

//utility function to get the container of an element by tagname
function AspNetTreeView_GetParentByTagName(parentTagName, childElementObj)
{
    var parent = childElementObj.parentNode;
    while(parent.tagName.toLowerCase() != parentTagName.toLowerCase())
    {
        parent = parent.parentNode;
    }
    return parent; 
}