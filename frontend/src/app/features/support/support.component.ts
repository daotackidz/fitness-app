import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import * as signalR from '@microsoft/signalr';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { AuthService } from '../../core/services/auth.service';

interface SupportTicket {
  id: string;
  userId: string;
  userFullName: string;
  subject: string;
  status: string;
  createdAt: string;
}

interface SupportMessage {
  id: string;
  ticketId: string;
  senderType: string;
  message: string;
  sentAt: string;
}

@Component({
  selector: 'app-support',
  standalone: true,
  imports: [CommonModule, FormsModule, MatListModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './support.component.html',
  styleUrl: './support.component.scss'
})
export class SupportComponent implements OnInit, OnDestroy {
  readonly tickets = signal<SupportTicket[]>([]);
  readonly selectedTicket = signal<SupportTicket | null>(null);
  readonly messages = signal<SupportMessage[]>([]);
  replyText = '';
  private hubConnection: signalR.HubConnection | null = null;

  constructor(private http: HttpClient, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }

  async loadTickets(): Promise<void> {
    const response = await firstValueFrom(
      this.http.get<ApiResponse<SupportTicket[]>>(`${environment.apiBaseUrl}/admin/support-tickets`)
    );
    this.tickets.set(response.data ?? []);
  }

  async selectTicket(ticket: SupportTicket): Promise<void> {
    this.selectedTicket.set(ticket);
    const response = await firstValueFrom(
      this.http.get<ApiResponse<SupportMessage[]>>(`${environment.apiBaseUrl}/support/tickets/${ticket.id}/messages`)
    );
    this.messages.set(response.data ?? []);
    await this.connectHub(ticket.id);
  }

  private async connectHub(ticketId: string): Promise<void> {
    await this.hubConnection?.stop();

    const hubBaseUrl = environment.apiBaseUrl.replace(/\/api$/, '');
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${hubBaseUrl}/hubs/support/${ticketId}`, {
        accessTokenFactory: () => this.authService.accessToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveMessage', (message: SupportMessage) => {
      this.messages.update((current) => [...current, message]);
    });

    await this.hubConnection.start();
  }

  async updateStatus(status: string): Promise<void> {
    const ticket = this.selectedTicket();
    if (!ticket) return;

    await firstValueFrom(this.http.patch(`${environment.apiBaseUrl}/admin/support-tickets/${ticket.id}`, { status }));
    ticket.status = status;
    this.loadTickets();
  }

  async sendReply(): Promise<void> {
    const ticket = this.selectedTicket();
    if (!ticket || !this.replyText.trim()) return;

    await firstValueFrom(
      this.http.post(`${environment.apiBaseUrl}/admin/support-tickets/${ticket.id}/messages`, { message: this.replyText })
    );
    this.replyText = '';
  }
}
