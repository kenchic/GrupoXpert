import json
from pathlib import Path
from collections import Counter

root = Path(r"e:\Documentos\Proyectos\GrupoXpert")

try:
    content = Path('.graphify_detect.json').read_text(encoding='utf-16')
except:
    content = Path('.graphify_detect.json').read_text(encoding='utf-8')
data = json.loads(content)

print('SUMMARY_START')
print(f'total_files: {data.get("total_files", 0)}')
print(f'total_words: {data.get("total_words", 0)}')
files_dict = data.get('files', {})
for k in ['code', 'document', 'paper', 'image', 'video']:
    if k in files_dict and files_dict[k]:
        print(f'{k}: {len(files_dict[k])} files')

# Get top 5 subdirectories relative to root
dirs = []
for k in files_dict:
    for f in files_dict[k]:
        try:
            rel_p = Path(f).relative_to(root)
            if len(rel_p.parts) > 1:
                dirs.append(rel_p.parts[0])
            else:
                dirs.append('.')
        except ValueError:
            dirs.append('EXTERNAL')

top_dirs = Counter(dirs).most_common(5)
print('TOP_DIRS_START')
for d, count in top_dirs:
    print(f'{d}: {count} files')
print('TOP_DIRS_END')
print('SUMMARY_END')
