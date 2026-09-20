import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

export interface BarChartItem {
  label: string;
  value: number;
  color: string;
}

@Component({
  selector: 'app-bar-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './bar-chart.component.html',
  styleUrl: './bar-chart.component.scss'
})
export class BarChartComponent {
  @Input() items: BarChartItem[] = [];
  @Input() emptyLabel = 'Chua co du lieu';

  get maxValue(): number {
    return Math.max(1, ...this.items.map((i) => i.value));
  }

  widthPercent(value: number): number {
    return this.maxValue === 0 ? 0 : Math.max(2, (value / this.maxValue) * 100);
  }

  formatValue(value: number): string {
    return value.toLocaleString('vi-VN');
  }
}
