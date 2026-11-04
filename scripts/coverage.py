import argparse
import json
from pathlib import Path
import xml.etree.ElementTree as ET

parser = argparse.ArgumentParser(description='Consolida cobertura de Domínio e Aplicação por linha de código.')
parser.add_argument('results', type=Path)
parser.add_argument('--minimum', type=float, default=80)
parser.add_argument('--output', type=Path)
args = parser.parse_args()
coverage = {name: {} for name in ('Dobu.Domain', 'Dobu.Application')}
reports = list(args.results.rglob('coverage.cobertura.xml'))
if not reports:
    raise SystemExit('Nenhum relatório Cobertura encontrado.')
for report in reports:
    for package in ET.parse(report).getroot().findall('./packages/package'):
        if package.attrib['name'] not in coverage:
            continue
        lines = coverage[package.attrib['name']]
        for cls in package.findall('./classes/class'):
            for line in cls.findall('./lines/line'):
                key = (cls.attrib['filename'], int(line.attrib['number']))
                lines[key] = max(lines.get(key, 0), int(line.attrib['hits']))
summary = {}
for name, lines in coverage.items():
    covered = sum(hits > 0 for hits in lines.values())
    summary[name] = {'coveredLines': covered, 'totalLines': len(lines), 'percent': round(100 * covered / len(lines), 2) if lines else 0}
result = json.dumps(summary, ensure_ascii=False, indent=2)
print(result)
if args.output:
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(result + '\n', encoding='utf-8')
if any(item['percent'] < args.minimum for item in summary.values()):
    raise SystemExit(f'Cobertura abaixo do limite interno de {args.minimum}%.')
