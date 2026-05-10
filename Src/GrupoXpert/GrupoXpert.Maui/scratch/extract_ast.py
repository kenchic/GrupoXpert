import sys, json
from graphify.extract import collect_files, extract
from pathlib import Path

try:
    with open('.graphify_detect.json', encoding='utf-16') as f:
        detect = json.load(f)
    
    code_files = [Path(f) for f in detect.get('files', {}).get('code', [])]

    if code_files:
        result = extract(code_files)
        with open('.graphify_ast.json', 'w') as f:
            json.dump(result, f, indent=2)
        print(f"AST: {len(result['nodes'])} nodes, {len(result['edges'])} edges")
    else:
        with open('.graphify_ast.json', 'w') as f:
            json.dump({'nodes':[],'edges':[],'input_tokens':0,'output_tokens':0}, f)
        print('No code files - skipping AST extraction')
except Exception as e:
    print(f"Error: {e}")
