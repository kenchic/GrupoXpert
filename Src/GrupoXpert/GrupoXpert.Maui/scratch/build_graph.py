import sys, json
from pathlib import Path
from graphify.build import build_from_json
from graphify.cluster import cluster, score_all
from graphify.analyze import god_nodes, surprising_connections, suggest_questions
from graphify.report import generate
from graphify.export import to_json, to_html
import os

try:
    ast = json.loads(Path('.graphify_ast.json').read_text(encoding='utf-8'))
    sem = json.loads(Path('graphify-out/.graphify_semantic.json').read_text(encoding='utf-8'))

    # Merge nodes
    seen = {n['id'] for n in ast['nodes']}
    merged_nodes = list(ast['nodes'])
    for n in sem['nodes']:
        if n['id'] not in seen:
            merged_nodes.append(n)
            seen.add(n['id'])

    merged_edges = ast['edges'] + sem['edges']
    merged = {
        'nodes': merged_nodes,
        'edges': merged_edges,
        'input_tokens': sem.get('input_tokens', 0),
        'output_tokens': sem.get('output_tokens', 0),
    }

    if not os.path.exists('graphify-out'):
        os.makedirs('graphify-out')

    # Build and cluster
    G = build_from_json(merged)
    communities = cluster(G)
    cohesion = score_all(G, communities)
    gods = god_nodes(G)
    surprises = surprising_connections(G, communities)
    
    # Simple labels for now
    labels = {cid: f"Community {cid}" for cid in communities}
    questions = suggest_questions(G, communities, labels)

    with open('.graphify_extract.json', 'w') as f:
        json.dump(merged, f, indent=2)

    to_json(G, communities, 'graphify-out/graph.json')
    to_html(G, communities, 'graphify-out/graph.html', community_labels=labels)
    
    # Save analysis for Step 5
    analysis = {
        'communities': {str(k): v for k, v in communities.items()},
        'cohesion': {str(k): v for k, v in cohesion.items()},
        'gods': gods,
        'surprises': surprises,
        'questions': questions
    }
    with open('.graphify_analysis.json', 'w') as f:
        json.dump(analysis, f, indent=2)

    print(f"Graph: {G.number_of_nodes()} nodes, {G.number_of_edges()} edges, {len(communities)} communities")

except Exception as e:
    print(f"Error: {e}")
