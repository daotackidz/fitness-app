import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { ApiResponse } from '../../core/models/api-response.model';
import { BarChartComponent, BarChartItem } from '../../shared/charts/bar-chart.component';
import { LineChartComponent, LineChartPoint } from '../../shared/charts/line-chart.component';

interface TopContent {
  id: string;
  name: string;
  score: number;
}

interface DashboardSummary {
  totalUsers: number;
  newUsersToday: number;
  newUsersThisWeek: number;
  openTickets: number;
  pendingReports: number;
  topRoutines: TopContent[];
  topArticles: TopContent[];
}

interface TimeSeriesPoint {
  date: string;
  count: number;
}

interface DistributionItem {
  label: string;
  count: number;
}

// Categorical palette (validated: dataviz skill, fixed order, never cycled).
const BLUE = '#2a78d6';
const ORANGE = '#eb6834';
const AQUA = '#1baf7a';
const YELLOW = '#eda100';
const MAGENTA = '#e87ba4';
const GREEN = '#008300';
const VIOLET = '#4a3aa7';

// Status palette (fixed, reserved for state - never reused as a series hue).
const STATUS_WARNING = '#fab219';
const STATUS_SERIOUS = '#ec835a';
const STATUS_GOOD = '#0ca30c';
const STATUS_MUTED = '#898781';

const TICKET_STATUS_COLORS: Record<string, string> = { Open: STATUS_WARNING, Pending: STATUS_SERIOUS, Closed: STATUS_GOOD };
const REPORT_STATUS_COLORS: Record<string, string> = { Pending: STATUS_WARNING, Reviewed: STATUS_GOOD, Dismissed: STATUS_MUTED };
const CONTENT_LABELS: Record<string, string> = {
  Exercises: 'Bai tap',
  Routines: 'Routine',
  'Meal Plans': 'Meal Plan',
  Articles: 'Article',
  Videos: 'Video',
  Faqs: 'FAQ',
  Challenges: 'Challenge'
};
const CONTENT_COLORS: Record<string, string> = {
  Exercises: BLUE,
  Routines: ORANGE,
  'Meal Plans': AQUA,
  Articles: YELLOW,
  Videos: MAGENTA,
  Faqs: GREEN,
  Challenges: VIOLET
};

function toSeriesPoints(series: TimeSeriesPoint[]): LineChartPoint[] {
  return series.map((p) => ({
    label: p.date.slice(5).split('-').reverse().join('/'),
    value: p.count
  }));
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, LineChartComponent, BarChartComponent, TranslatePipe],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  readonly summary = signal<DashboardSummary | null>(null);
  readonly userGrowth = signal<LineChartPoint[]>([]);
  readonly workoutActivity = signal<LineChartPoint[]>([]);
  readonly contentDistribution = signal<BarChartItem[]>([]);
  readonly ticketStatus = signal<BarChartItem[]>([]);
  readonly reportStatus = signal<BarChartItem[]>([]);
  readonly topRoutines = signal<BarChartItem[]>([]);
  readonly topArticles = signal<BarChartItem[]>([]);

  constructor(private http: HttpClient) {}

  async ngOnInit(): Promise<void> {
    const base = `${environment.apiBaseUrl}/admin/dashboard`;
    const [summary, userGrowth, workoutActivity, contentDistribution, ticketStatus, reportStatus] = await Promise.all([
      firstValueFrom(this.http.get<ApiResponse<DashboardSummary>>(`${base}/summary`)),
      firstValueFrom(this.http.get<ApiResponse<TimeSeriesPoint[]>>(`${base}/user-growth`, { params: { days: 30 } })),
      firstValueFrom(this.http.get<ApiResponse<TimeSeriesPoint[]>>(`${base}/workout-activity`, { params: { days: 30 } })),
      firstValueFrom(this.http.get<ApiResponse<DistributionItem[]>>(`${base}/content-distribution`)),
      firstValueFrom(this.http.get<ApiResponse<DistributionItem[]>>(`${base}/ticket-status`)),
      firstValueFrom(this.http.get<ApiResponse<DistributionItem[]>>(`${base}/report-status`))
    ]);

    this.summary.set(summary.data);
    this.userGrowth.set(toSeriesPoints(userGrowth.data ?? []));
    this.workoutActivity.set(toSeriesPoints(workoutActivity.data ?? []));

    this.contentDistribution.set(
      (contentDistribution.data ?? []).map((item) => ({
        label: CONTENT_LABELS[item.label] ?? item.label,
        value: item.count,
        color: CONTENT_COLORS[item.label] ?? BLUE
      }))
    );
    this.ticketStatus.set(
      (ticketStatus.data ?? []).map((item) => ({ label: item.label, value: item.count, color: TICKET_STATUS_COLORS[item.label] ?? STATUS_MUTED }))
    );
    this.reportStatus.set(
      (reportStatus.data ?? []).map((item) => ({ label: item.label, value: item.count, color: REPORT_STATUS_COLORS[item.label] ?? STATUS_MUTED }))
    );
    this.topRoutines.set((summary.data?.topRoutines ?? []).map((item) => ({ label: item.name, value: item.score, color: BLUE })));
    this.topArticles.set((summary.data?.topArticles ?? []).map((item) => ({ label: item.name, value: item.score, color: BLUE })));
  }
}
