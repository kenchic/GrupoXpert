import json, glob
from pathlib import Path

# Step 3B Part 3: Merge chunks
chunks = sorted(glob.glob('graphify-out/.graphify_chunk_*.json'))
all_nodes, all_edges, all_hyperedges = [], [], []
total_in, total_out = 0, 0
for c in chunks:
    try:
        d = json.loads(Path(c).read_text())
        all_nodes += d.get('nodes', [])
        all_edges += d.get('edges', [])
        all_hyperedges += d.get('hyperedges', [])
        total_in += d.get('input_tokens', 0)
        total_out += d.get('output_tokens', 0)
    except Exception as e:
        print(f"Error reading chunk {c}: {e}")

merged_semantic = {
    'nodes': all_nodes,
    'edges': all_edges,
    'hyperedges': all_hyperedges,
    'input_tokens': total_in,
    'output_tokens': total_out,
}
Path('.graphify_semantic.json').write_text(json.dumps(merged_semantic, indent=2))
print(f'Merged {len(chunks)} chunks into .graphify_semantic.json')

# Step 3C: Merge AST + Semantic
ast_path = Path('.graphify_ast.json')
if ast_path.exists():
    ast = json.loads(ast_path.read_text())
else:
    ast = {'nodes': [], 'edges': [], 'input_tokens': 0, 'output_tokens': 0}

seen = {n['id'] for n in ast['nodes']}
merged_nodes = list(ast['nodes'])
for n in merged_semantic['nodes']:
    if n['id'] not in seen:
        merged_nodes.append(n)
        seen.add(n['id'])

merged_edges = ast['edges'] + merged_semantic['edges']
merged_hyperedges = merged_semantic.get('hyperedges', [])

final_extract = {
    'nodes': merged_nodes,
    'edges': merged_edges,
    'hyperedges': merged_hyperedges,
    'input_tokens': merged_semantic.get('input_tokens', 0),
    'output_tokens': merged_semantic.get('output_tokens', 0),
}
Path('.graphify_extract.json').write_text(json.dumps(final_extract, indent=2))
print(f'Final merge: {len(merged_nodes)} nodes, {len(merged_edges)} edges')
