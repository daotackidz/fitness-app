import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

export interface LineChartPoint {
  label: string;
  value: number;
}

const PAD_L = 8;
const PAD_R = 34;
const PAD_T = 12;
const PAD_B = 18;
const PLOT_W = 278;
const VIEW_H = 100;

@Component({
  selector: 'app-line-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './line-chart.component.html',
  styleUrl: './line-chart.component.scss'
})
export class LineChartComponent {
  @Input() points: LineChartPoint[] = [];
  @Input() color = '#2a78d6';
  @Input() emptyLabel = 'Chua co du lieu';

  readonly viewBox = `0 0 ${PAD_L + PLOT_W + PAD_R} ${VIEW_H}`;
  readonly baselineY = PAD_T + (VIEW_H - PAD_T - PAD_B);

  private get valueRange(): { min: number; max: number } {
    const values = this.points.map((p) => p.value);
    let min = Math.min(...values);
    let max = Math.max(...values);
    if (min === max) {
      min -= 1;
      max += 1;
    }
    return { min, max };
  }

  private x(index: number): number {
    if (this.points.length <= 1) return PAD_L;
    return PAD_L + (index * PLOT_W) / (this.points.length - 1);
  }

  private y(value: number): number {
    const { min, max } = this.valueRange;
    const plotH = VIEW_H - PAD_T - PAD_B;
    return PAD_T + plotH - ((value - min) / (max - min)) * plotH;
  }

  get linePath(): string {
    return this.points.map((p, i) => `${i === 0 ? 'M' : 'L'} ${this.x(i)},${this.y(p.value)}`).join(' ');
  }

  get areaPath(): string {
    if (this.points.length === 0) return '';
    const first = this.x(0);
    const last = this.x(this.points.length - 1);
    return `${this.linePath} L ${last},${this.baselineY} L ${first},${this.baselineY} Z`;
  }

  get lastPoint(): { x: number; y: number; value: number } | null {
    if (this.points.length === 0) return null;
    const idx = this.points.length - 1;
    return { x: this.x(idx), y: this.y(this.points[idx].value), value: this.points[idx].value };
  }

  get hoverPoints(): { x: number; y: number; label: string; value: number }[] {
    return this.points.map((p, i) => ({ x: this.x(i), y: this.y(p.value), label: p.label, value: p.value }));
  }

  get firstLabel(): string {
    return this.points[0]?.label ?? '';
  }

  get lastLabel(): string {
    return this.points[this.points.length - 1]?.label ?? '';
  }

  formatValue(value: number): string {
    return value.toLocaleString('vi-VN');
  }
}
