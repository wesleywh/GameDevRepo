import os,sys, argparse,shutil,re
from functools import reduce
#====================================
# Will build my documentation site based on notes and
# variables inside my scripts.
#====================================
def main():
    parser = argparse.ArgumentParser(description='Supply path to root folder of scripts and will build docs webpages')
    parser.add_argument("-p","--path",dest="path",help="Full path to root of scripts directory to scan.")
    parser.add_argument("-m","--main",dest="main",help="The main page, full http path. EX: https://wesleywh.github.io/GameDevRepo/")
    args = parser.parse_args()

    if args.main == None:
        args.main = "https://wesleywh.github.io/GameDevRepo/"
    build_var_pages(args)
    build_navigation_page(args)

def build_navigation_page(args):
    cur_dir = os.path.dirname(os.path.realpath(__file__))
    tree=""
    navigation={}
    for fullpath, dirs, files in os.walk(os.path.join(cur_dir,"docs")):
        for filename in files:
            parts=os.path.join(fullpath,filename).replace(cur_dir+"%sdocs%s"%(os.sep,os.sep),"").split(os.sep)
            docs_path=os.path.join(fullpath,filename).replace(cur_dir+"%s"%os.sep,"").replace("docs%s"%os.sep,"",1)
            if len(parts) < 2:
                parts=["Miscellaneous",parts[0]]
            if parts[0] not in navigation:
                navigation[parts[0]]=[]
            navigation[parts[0]].append("[%s](%s)"%(parts[-1].replace(".md",""),docs_path.replace(os.sep,"/")))

    with open(os.path.join(cur_dir,"docs","navigation.md"),"w") as f:
        f.write("[Back To Main Page](%s)\n\n"%args.main)
        f.write("# Navigation Tree\n")
        f.write("Find documentation relating to various script files found in this repository here.\n")
        f.write("This is automatically generated based on comments within the script itself.\n")
        f.write("If there is missing data it is because a comment has not been made in that file.\n\n")
        cur_key=""
        for key,value in navigation.items():
            if cur_key != key:
                cur_key=key
                f.write("\n### %s\n"%key)
            for item in value:
                f.write("  + %s\n"%item)

def build_var_pages(args):
    cur_dir = os.path.dirname(os.path.realpath(__file__))
    for fullpath, dirs, files in os.walk(args.path, topdown=True):
        for filename in files:
            if (filename.endswith(".cs") and "InputManager" not in fullpath and
             "PatchLib" not in fullpath and "Vehicles" not in fullpath):
                with open(os.path.join(fullpath,filename)) as f:
                    contents = f.readlines()
                description = extract_file_description(contents)
                variables = extract_variables(contents)
                sep=os.pathsep
                final_path = os.path.join(fullpath,filename).replace(args.path,"").replace(".cs",".md",-1).lstrip("/").lstrip("\\\\")
                script_title = filename.replace(".cs","",-1)
                part1=os.path.join(cur_dir,"docs")
                part2=final_path
                save_path=os.path.join(part1,part2)
                dir_path=os.path.dirname(save_path)
                if not os.path.exists(dir_path):
                    os.makedirs(dir_path)
                with open(save_path,'w') as save_file:
                    save_file.write("[Back To Navigation Tree](%sdocs/navigation.html)\n"%args.main)
                    save_file.write("# %s"%script_title)
                    save_file.write("\n\n")
                    save_file.write("## Description:\n")
                    save_file.write(description)
                    save_file.write("\n\n")
                    save_file.write("## Variables:\n")
                    save_file.write("List of variables that you can modify in the inspector.\n\n")
                    save_file.write("|Access|Name|Type|Default Value|Description|\n")
                    save_file.write("|---|---|---|---|---|\n")
                    for var in variables:
                        save_file.write("|%s|%s|%s|%s|%s|\n"%(var["ACCESS"],var["NAME"],var["TYPE"],var["DEFAULT"],var["DESC"]))

#Variable Parsing
def extract_variables(contents):
    variables=[]
    regex="((public|private)?.+?[ ].+?([ ])?\(.+?\))"
    for line in contents:
        if ("SerializeField" in line or ("public" in line and "class" not in line) and
        "void" not in line and re.match(regex,line) is None):
            variables.append({
                "ACCESS":get_access(line),
                "TYPE":get_type(line),
                "NAME":get_name(line),
                "DEFAULT":get_default(line),
                "DESC":get_description(line)})

    return variables

def get_access(line):
    line=line.strip()
    retVal=""
    if "SerializeField" in line:
        line=line.split("SerializeField")[1].replace("]","").strip()
    if "public" in line:
        retVal="public"
    elif "private" in line:
        retVal="private"
    else:
        retVal="private"
    return retVal

def get_description(line):
    line=line.strip()
    retVal=""
    val = line.split("//")
    if len(val) > 1:
        retVal = val[1].strip().lstrip("//")
    else:
        retVal="No description."
    return retVal

def get_type(line):
    retVal=line.strip()
    if "SerializeField" in retVal:
        retVal=line.split("SerializeField")[1].replace("]","",1).strip()

    if "public" in retVal:
        retVal = retVal.split("public")[1].strip()
        retVal = retVal.split(" ")[0].strip()
    elif "private" in retVal:
        retVal = retVal.split("private")[1].strip()
        retVal = retVal.split(" ")[0].strip()
    else:
        retVal = line.split(" ")[0].strip()
    return retVal

def get_name(line):
    line=line.strip()
    retVal=line.split(get_type(line))[1].strip().split("=")[0].strip()
    retVal=retVal.split("#")[0].strip().replace(";","")
    return retVal

def get_default(line):
    retVal=""
    if "#" in line:
        retVal=line.split("#")[0].strip()
    else:
        retVal=line.strip()

    if "=" in retVal:
        retVal=retVal.split("=")[1].strip().split(";")[0].strip().split(" ")[-1].strip()
    else:
        retVal="no default"

    return retVal

#File description parsing
def extract_file_description(contents):
    inDescription=False
    retVal=""
    description=[]
    for line in contents:
        if inDescription == True:
            if "</summary>" in line:
                break
            description.append(line.replace("///","",1).strip())
        elif "using" in line and inDescription==False:
            retVal="File has no description."
            break
        elif "summary" in line:
            inDescription=True
    if len(description) > 0:
        content=""
        for line in description:
            if line == "":
                content+="\n\n"
            else:
                content+=" "+line
        retVal=content
    return retVal

main()
