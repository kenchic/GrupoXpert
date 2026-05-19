import json
from pathlib import Path

inc_path = Path(".graphify_incremental.json")
if not inc_path.exists():
    print("Error: .graphify_incremental.json not found")
    exit(1)

data = json.loads(inc_path.read_text(encoding="utf-8"))

def should_keep(filepath):
    path_lower = filepath.lower()
    # Exclude standard build directories
    if "\\obj\\" in path_lower or "/obj/" in path_lower:
        return False
    if "\\bin\\" in path_lower or "/bin/" in path_lower:
        return False
    if "\\.vs\\" in path_lower or "/.vs/" in path_lower:
        return False
    if "\\.git\\" in path_lower or "/.git/" in path_lower:
        return False
    if "\\.vscode\\" in path_lower or "/.vscode/" in path_lower:
        return False
    if "\\resizetizer\\" in path_lower or "/resizetizer/" in path_lower:
        return False
    
    # Keep only files in src or docs (or root config files like README/gitignore if needed)
    is_in_src_or_docs = "src" in path_lower or "docs" in path_lower
    
    # Exclude some asset files we don't care about in graph
    if "logo" in path_lower or "appicon" in path_lower or "dotnet_bot" in path_lower or "splash" in path_lower:
        return False
        
    return is_in_src_or_docs

new_files = data.get("new_files", {})
filtered_new_files = {}
total_filtered = 0

for category, files in new_files.items():
    filtered_list = [f for f in files if should_keep(f)]
    filtered_new_files[category] = filtered_list
    total_filtered += len(filtered_list)

data["new_files"] = filtered_new_files
data["new_total"] = total_filtered

# Write it back
inc_path.write_text(json.dumps(data, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"Filtered .graphify_incremental.json: kept {total_filtered} files.")
