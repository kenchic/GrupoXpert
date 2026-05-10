import sys, json
from graphify.build import build_from_json
from graphify.cluster import score_all
from graphify.analyze import god_nodes, surprising_connections, suggest_questions
from graphify.report import generate
from pathlib import Path

try:
    extraction = json.loads(Path('.graphify_extract.json').read_text(encoding='utf-8'))
    detection  = json.loads(Path('.graphify_detect.json').read_text(encoding='utf-16'))
    analysis   = json.loads(Path('.graphify_analysis.json').read_text(encoding='utf-8'))

    G = build_from_json(extraction)
    communities = {int(k): v for k, v in analysis['communities'].items()}
    cohesion = {int(k): v for k, v in analysis['cohesion'].items()}
    tokens = {'input': extraction.get('input_tokens', 0), 'output': extraction.get('output_tokens', 0)}

    labels = {
        0: "Apple Platforms Setup",
        1: "MAUI App Lifecycle",
        2: "Android Application Base",
        3: "Windows Desktop Platform",
        4: "Main UI Entry (MainPage)",
        5: "MauiProgram Bootstrapper",
        6: "Android Activity Lifecycle",
        7: "iOS Program Runner",
        8: "MacCatalyst Program Runner",
        9: "Icon Rendering Audit",
        10: "Notch & Safe Area Audit",
        11: "Dev Scripts & Tooling"
    }

    questions = suggest_questions(G, communities, labels)

    report = generate(G, communities, cohesion, labels, analysis['gods'], analysis['surprises'], detection, tokens, '.', suggested_questions=questions)
    Path('graphify-out/GRAPH_REPORT.md').write_text(report, encoding='utf-8')
    Path('.graphify_labels.json').write_text(json.dumps({str(k): v for k, v in labels.items()}), encoding='utf-8')
    print('Report updated with community labels')
except Exception as e:
    print(f"Error: {e}")
