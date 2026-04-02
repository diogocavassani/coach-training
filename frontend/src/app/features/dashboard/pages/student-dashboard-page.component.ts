import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Chart } from 'chart.js/auto';
import { jsPDF } from 'jspdf';
import * as XLSX from 'xlsx';

import { DashboardAtleta, DashboardTreinoJanela } from '../models/dashboard.model';
import { DashboardApiService } from '../services/dashboard-api.service';
import { StudentsApiService } from '../../students/services/students-api.service';

@Component({
  selector: 'app-student-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule
  ],
  templateUrl: './student-dashboard-page.component.html',
  styleUrl: './student-dashboard-page.component.css'
})
export class StudentDashboardPageComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('cargaChart') cargaChartRef?: ElementRef<HTMLCanvasElement>;
  @ViewChild('paceChart') paceChartRef?: ElementRef<HTMLCanvasElement>;

  dashboard: DashboardAtleta | null = null;
  carregando = true;
  mensagemErro = '';
  mensagemErroProvaAlvo = '';
  carregandoProvaAlvo = true;
  salvandoProvaAlvo = false;
  mensagemErroPlanejamentoBase = '';
  carregandoPlanejamentoBase = true;
  salvandoPlanejamentoBase = false;

  readonly provaAlvoForm;
  readonly planejamentoBaseForm;

  private chartCarga?: Chart;
  private chartPace?: Chart;
  private viewInicializada = false;
  private atletaId: string | null = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly dashboardApiService: DashboardApiService,
    private readonly studentsApiService: StudentsApiService,
    private readonly snackBar: MatSnackBar
  ) {
    this.provaAlvoForm = this.formBuilder.group({
      dataProva: ['', [Validators.required]],
      distanciaKm: [null as number | null, [Validators.required, Validators.min(0.1)]],
      objetivo: ['']
    });
    this.planejamentoBaseForm = this.formBuilder.group({
      treinosPlanejadosPorSemana: [null as number | null, [Validators.required, Validators.min(1), Validators.max(14)]]
    });
  }

  ngOnInit(): void {
    this.atletaId = this.route.snapshot.paramMap.get('id');
    if (!this.atletaId) {
      this.carregando = false;
      this.carregandoProvaAlvo = false;
      this.carregandoPlanejamentoBase = false;
      this.mensagemErro = 'Nao foi possivel identificar o aluno para exibir o dashboard.';
      return;
    }

    this.carregarDashboard(this.atletaId);
    this.carregarProvaAlvo(this.atletaId);
    this.carregarPlanejamentoBase(this.atletaId);
  }

  ngAfterViewInit(): void {
    this.viewInicializada = true;
    this.atualizarGraficos();
  }

  ngOnDestroy(): void {
    this.destruirGraficos();
  }

  get statusRiscoDescricao(): string {
    if (!this.dashboard) {
      return '-';
    }

    switch (this.dashboard.statusRisco) {
      case 0:
        return 'Normal';
      case 1:
        return 'Atencao';
      case 2:
        return 'Risco';
      default:
        return 'Nao definido';
    }
  }

  get statusRiscoClasse(): string {
    if (!this.dashboard) {
      return '';
    }

    switch (this.dashboard.statusRisco) {
      case 0:
        return 'risco-normal';
      case 1:
        return 'risco-atencao';
      case 2:
        return 'risco-alto';
      default:
        return '';
    }
  }

  get faseDescricao(): string {
    if (!this.dashboard) {
      return '-';
    }

    switch (this.dashboard.faseAtual) {
      case 0:
        return 'Base';
      case 1:
        return 'Construcao';
      case 2:
        return 'Pico';
      case 3:
        return 'Polimento';
      default:
        return 'Nao definida';
    }
  }

  get podeExportar(): boolean {
    return !!this.dashboard && this.dashboard.treinosJanela.length > 0;
  }

  salvarProvaAlvo(): void {
    if (!this.atletaId) {
      return;
    }

    if (this.provaAlvoForm.invalid) {
      this.provaAlvoForm.markAllAsTouched();
      return;
    }

    const valor = this.provaAlvoForm.getRawValue();
    this.salvandoProvaAlvo = true;
    this.mensagemErroProvaAlvo = '';

    this.studentsApiService
      .salvarProvaAlvo(this.atletaId, {
        dataProva: valor.dataProva ?? '',
        distanciaKm: Number(valor.distanciaKm),
        objetivo: valor.objetivo?.trim() || undefined
      })
      .pipe(finalize(() => (this.salvandoProvaAlvo = false)))
      .subscribe({
        next: (provaAlvo) => {
          this.provaAlvoForm.patchValue({
            dataProva: provaAlvo.dataProva,
            distanciaKm: provaAlvo.distanciaKm,
            objetivo: provaAlvo.objetivo ?? ''
          });
          this.snackBar.open('Prova alvo salva com sucesso.', 'Fechar', { duration: 3000 });
          this.carregarDashboard(this.atletaId!);
        },
        error: (error: HttpErrorResponse) => {
          this.mensagemErroProvaAlvo = error.error?.erro ?? 'Nao foi possivel salvar a prova alvo.';
        }
      });
  }

  salvarPlanejamentoBase(): void {
    if (!this.atletaId) {
      return;
    }

    if (this.planejamentoBaseForm.invalid) {
      this.planejamentoBaseForm.markAllAsTouched();
      return;
    }

    const valor = this.planejamentoBaseForm.getRawValue();
    this.salvandoPlanejamentoBase = true;
    this.mensagemErroPlanejamentoBase = '';

    this.studentsApiService
      .salvarPlanejamentoBase(this.atletaId, {
        treinosPlanejadosPorSemana: Number(valor.treinosPlanejadosPorSemana)
      })
      .pipe(finalize(() => (this.salvandoPlanejamentoBase = false)))
      .subscribe({
        next: (planejamentoBase) => {
          this.planejamentoBaseForm.patchValue({
            treinosPlanejadosPorSemana: planejamentoBase.treinosPlanejadosPorSemana
          });
          this.snackBar.open('Planejamento base salvo com sucesso.', 'Fechar', { duration: 3000 });
          this.carregarDashboard(this.atletaId!);
        },
        error: (error: HttpErrorResponse) => {
          this.mensagemErroPlanejamentoBase = error.error?.erro ?? 'Nao foi possivel salvar o planejamento base.';
        }
      });
  }

  exportarExcel(): void {
    if (!this.dashboard) {
      return;
    }

    try {
      const linhas = this.dashboard.treinosJanela.map((treino) => ({
        Data: this.formatarData(treino.data),
        Tipo: this.obterTipoTreinoDescricao(treino.tipo),
        'Duracao (min)': treino.duracaoMinutos,
        'Distancia (km)': treino.distanciaKm,
        RPE: treino.rpe,
        Carga: treino.carga,
        'Pace (min/km)': treino.paceMinPorKm != null ? treino.paceMinPorKm.toFixed(2) : '-'
      }));

      const worksheet = XLSX.utils.json_to_sheet(linhas);
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, 'Treinos');
      this.salvarWorkbook(workbook, this.gerarNomeArquivo('xlsx'));

      this.snackBar.open('Excel exportado com sucesso.', 'Fechar', { duration: 3000 });
    } catch {
      this.snackBar.open('Nao foi possivel exportar o Excel.', 'Fechar', { duration: 4000 });
    }
  }

  exportarPdf(): void {
    if (!this.dashboard) {
      return;
    }

    try {
      const doc = new jsPDF({ unit: 'pt', format: 'a4' });
      const margemEsquerda = 40;
      let y = 46;

      doc.setFontSize(18);
      doc.text(`Dashboard do aluno - ${this.dashboard.nome}`, margemEsquerda, y);
      y += 24;

      doc.setFontSize(10);
      doc.text(`Gerado em ${new Date().toLocaleString('pt-BR')}`, margemEsquerda, y);
      y += 22;

      doc.setFontSize(12);
      doc.text(`Carga semanal: ${this.dashboard.cargaSemanal}`, margemEsquerda, y);
      y += 16;
      doc.text(`Carga cronica: ${this.dashboard.cargaCronica}`, margemEsquerda, y);
      y += 16;
      doc.text(`ACWR: ${this.dashboard.acwr.toFixed(2)}`, margemEsquerda, y);
      y += 16;
      doc.text(`Fase: ${this.faseDescricao} | Risco: ${this.statusRiscoDescricao}`, margemEsquerda, y);
      y += 24;

      doc.setFontSize(13);
      doc.text('Insights', margemEsquerda, y);
      y += 16;
      doc.setFontSize(11);

      if (this.dashboard.insights.length === 0) {
        doc.text('- Sem insights no periodo.', margemEsquerda, y);
        y += 16;
      } else {
        this.dashboard.insights.forEach((insight) => {
          const linhas = doc.splitTextToSize(`- ${insight}`, 500);
          linhas.forEach((linha: string) => {
            y = this.garantirEspacoParaLinha(doc, y, 16);
            doc.text(linha, margemEsquerda, y);
            y += 14;
          });
          y += 2;
        });
      }

      y += 8;
      y = this.garantirEspacoParaLinha(doc, y, 36);
      doc.setFontSize(13);
      doc.text('Treinos da janela (12 semanas)', margemEsquerda, y);
      y += 16;

      doc.setFontSize(10);
      doc.text('Data', margemEsquerda, y);
      doc.text('Tipo', margemEsquerda + 72, y);
      doc.text('Duracao', margemEsquerda + 165, y);
      doc.text('Dist.', margemEsquerda + 240, y);
      doc.text('RPE', margemEsquerda + 292, y);
      doc.text('Carga', margemEsquerda + 338, y);
      doc.text('Pace', margemEsquerda + 390, y);
      y += 12;
      doc.line(margemEsquerda, y, 550, y);
      y += 12;

      this.dashboard.treinosJanela.forEach((treino) => {
        y = this.garantirEspacoParaLinha(doc, y, 20);
        doc.text(this.formatarData(treino.data), margemEsquerda, y);
        doc.text(this.obterTipoTreinoDescricao(treino.tipo), margemEsquerda + 72, y);
        doc.text(`${treino.duracaoMinutos} min`, margemEsquerda + 165, y);
        doc.text(`${treino.distanciaKm.toFixed(1)} km`, margemEsquerda + 240, y);
        doc.text(`${treino.rpe}`, margemEsquerda + 292, y);
        doc.text(`${treino.carga}`, margemEsquerda + 338, y);
        doc.text(treino.paceMinPorKm != null ? `${treino.paceMinPorKm.toFixed(2)}` : '-', margemEsquerda + 390, y);
        y += 18;
      });

      this.salvarDocumentoPdf(doc, this.gerarNomeArquivo('pdf'));
      this.snackBar.open('PDF exportado com sucesso.', 'Fechar', { duration: 3000 });
    } catch {
      this.snackBar.open('Nao foi possivel exportar o PDF.', 'Fechar', { duration: 4000 });
    }
  }

  formatarData(dataIso: string | null | undefined): string {
    if (!dataIso) {
      return '-';
    }

    const partes = dataIso.split('-');
    if (partes.length !== 3) {
      return '-';
    }

    const [ano, mes, dia] = partes;
    return `${dia.padStart(2, '0')}/${mes.padStart(2, '0')}/${ano}`;
  }

  obterTipoTreinoDescricao(tipo: number): string {
    switch (tipo) {
      case 0:
        return 'Leve';
      case 1:
        return 'Ritmo';
      case 2:
        return 'Intervalado';
      case 3:
        return 'Longo';
      default:
        return 'Nao definido';
    }
  }

  private carregarDashboard(atletaId: string): void {
    this.carregando = true;
    this.mensagemErro = '';

    this.dashboardApiService
      .obterPorAtletaId(atletaId)
      .pipe(finalize(() => (this.carregando = false)))
      .subscribe({
        next: (dashboard) => {
          this.dashboard = dashboard;
          this.atualizarGraficos();
        },
        error: (error: HttpErrorResponse) => {
          this.dashboard = null;
          this.mensagemErro = error.error?.erro ?? 'Nao foi possivel carregar o dashboard do aluno.';
        }
      });
  }

  private carregarProvaAlvo(atletaId: string): void {
    this.carregandoProvaAlvo = true;
    this.mensagemErroProvaAlvo = '';

    this.studentsApiService
      .obterProvaAlvo(atletaId)
      .pipe(finalize(() => (this.carregandoProvaAlvo = false)))
      .subscribe({
        next: (provaAlvo) => {
          this.provaAlvoForm.patchValue({
            dataProva: provaAlvo.dataProva,
            distanciaKm: provaAlvo.distanciaKm,
            objetivo: provaAlvo.objetivo ?? ''
          });
        },
        error: (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.provaAlvoForm.reset({
              dataProva: '',
              distanciaKm: null,
              objetivo: ''
            });
            return;
          }

          this.mensagemErroProvaAlvo = error.error?.erro ?? 'Nao foi possivel carregar a prova alvo.';
        }
      });
  }

  private carregarPlanejamentoBase(atletaId: string): void {
    this.carregandoPlanejamentoBase = true;
    this.mensagemErroPlanejamentoBase = '';

    this.studentsApiService
      .obterPlanejamentoBase(atletaId)
      .pipe(finalize(() => (this.carregandoPlanejamentoBase = false)))
      .subscribe({
        next: (planejamentoBase) => {
          this.planejamentoBaseForm.patchValue({
            treinosPlanejadosPorSemana: planejamentoBase.treinosPlanejadosPorSemana
          });
        },
        error: (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.planejamentoBaseForm.reset({
              treinosPlanejadosPorSemana: null
            });
            return;
          }

          this.mensagemErroPlanejamentoBase = error.error?.erro ?? 'Nao foi possivel carregar o planejamento base.';
        }
      });
  }

  private atualizarGraficos(): void {
    if (!this.viewInicializada || !this.dashboard || !this.cargaChartRef || !this.paceChartRef) {
      return;
    }

    this.destruirGraficos();

    const labels = this.dashboard.serieCargaSemanal.map((ponto) =>
      `${this.formatarData(ponto.semanaInicio)} - ${this.formatarData(ponto.semanaFim)}`
    );

    this.chartCarga = new Chart(this.cargaChartRef.nativeElement, {
      type: 'line',
      data: {
        labels,
        datasets: [
          {
            label: 'Carga semanal',
            data: this.dashboard.serieCargaSemanal.map((ponto) => ponto.valor),
            borderColor: '#22C55E',
            backgroundColor: 'rgba(34, 197, 94, 0.16)',
            fill: true,
            tension: 0.3
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: { color: '#F9FAFB' }
          }
        },
        scales: {
          x: {
            ticks: { color: '#9CA3AF', maxRotation: 0, autoSkip: true, maxTicksLimit: 6 },
            grid: { color: 'rgba(55, 65, 81, 0.35)' }
          },
          y: {
            ticks: { color: '#9CA3AF' },
            grid: { color: 'rgba(55, 65, 81, 0.35)' }
          }
        }
      }
    });

    this.chartPace = new Chart(this.paceChartRef.nativeElement, {
      type: 'line',
      data: {
        labels,
        datasets: [
          {
            label: 'Pace medio (min/km)',
            data: this.dashboard.seriePaceSemanal.map((ponto) => ponto.valorMinPorKm),
            borderColor: '#3B82F6',
            backgroundColor: 'rgba(59, 130, 246, 0.16)',
            fill: true,
            spanGaps: false,
            tension: 0.3
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: { color: '#F9FAFB' }
          }
        },
        scales: {
          x: {
            ticks: { color: '#9CA3AF', maxRotation: 0, autoSkip: true, maxTicksLimit: 6 },
            grid: { color: 'rgba(55, 65, 81, 0.35)' }
          },
          y: {
            ticks: { color: '#9CA3AF' },
            grid: { color: 'rgba(55, 65, 81, 0.35)' }
          }
        }
      }
    });
  }

  private destruirGraficos(): void {
    this.chartCarga?.destroy();
    this.chartPace?.destroy();
    this.chartCarga = undefined;
    this.chartPace = undefined;
  }

  private gerarNomeArquivo(extensao: 'xlsx' | 'pdf'): string {
    const nomeBase = this.dashboard?.nome
      .toLowerCase()
      .normalize('NFD')
      .replace(/[^\w\s-]/g, '')
      .replace(/\s+/g, '-')
      .trim();

    return `dashboard-${nomeBase || 'aluno'}-12-semanas.${extensao}`;
  }

  private garantirEspacoParaLinha(doc: jsPDF, yAtual: number, alturaNecessaria: number): number {
    const limiteInferior = 790;
    if (yAtual + alturaNecessaria <= limiteInferior) {
      return yAtual;
    }

    doc.addPage();
    return 46;
  }

  private salvarWorkbook(workbook: XLSX.WorkBook, nomeArquivo: string): void {
    XLSX.writeFile(workbook, nomeArquivo);
  }

  private salvarDocumentoPdf(doc: jsPDF, nomeArquivo: string): void {
    doc.save(nomeArquivo);
  }
}
