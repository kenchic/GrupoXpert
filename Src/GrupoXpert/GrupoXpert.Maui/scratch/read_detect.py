import json
import sys

try:
    with open('.graphify_detect.json', encoding='utf-16') as f:
        d = json.load(f)
    
    print(f"Total files: {d.get('total_files', 0)}")
    print(f"Total words: {d.get('total_words', 0)}")
    print("Files by type:")
    print(json.dumps(d.get('file_types', {}), indent=2))
except Exception as e:
    print(f"Error: {e}")
