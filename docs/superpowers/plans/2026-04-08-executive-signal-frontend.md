# Executive Signal Frontend Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rebuild the CoachTraining frontend around the approved Executive Signal design system, starting with the landing page and propagating the new premium, light, usability-first language through login and the authenticated product surfaces.

**Architecture:** Centralize the redesign in shared theme tokens inside `frontend/src/styles.css`, then update each user-facing surface in rollout order so the visual language is born in the public experience and carried into the product workspace. Keep business logic stable, prefer HTML/CSS/template refactors over TypeScript churn, and add or update focused Angular component tests that lock the new information hierarchy in place.

**Tech Stack:** Angular 19 standalone components, Angular Material, global CSS tokens, Karma/Jasmine, existing frontend docs in `docs/frontend`

---

## File Structure

**Create**
- `frontend/src/app/core/layout/app-shell.component.spec.ts`

**Modify**
- `frontend/src/index.html`
- `frontend/src/styles.css`
- `docs/frontend/design-system.md`
- `frontend/src/app/features/professor/pages/professor-landing-page.component.html`
- `frontend/src/app/features/professor/pages/professor-landing-page.component.css`
- `frontend/src/app/features/professor/pages/professor-landing-page.component.spec.ts`
- `frontend/src/app/features/auth/pages/login-page.component.html`
- `frontend/src/app/features/auth/pages/login-page.component.css`
- `frontend/src/app/features/auth/pages/login-page.component.spec.ts`
- `frontend/src/app/core/layout/app-shell.component.ts`
- `frontend/src/app/core/layout/app-shell.component.html`
- `frontend/src/app/core/layout/app-shell.component.css`
- `frontend/src/app/features/dashboard/pages/dashboard-page.component.html`
- `frontend/src/app/features/dashboard/pages/dashboard-page.component.css`
- `frontend/src/app/features/dashboard/pages/dashboard-page.component.spec.ts`
- `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.html`
- `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.css`
- `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.spec.ts`
- `frontend/src/app/features/students/pages/students-list-page.component.html`
- `frontend/src/app/features/students/pages/students-list-page.component.css`
- `frontend/src/app/features/students/pages/students-list-page.component.spec.ts`
- `frontend/src/app/features/students/pages/student-create-page.component.html`
- `frontend/src/app/features/students/pages/student-create-page.component.css`
- `frontend/src/app/features/students/pages/student-create-page.component.spec.ts`
- `frontend/src/app/features/trainings/pages/training-create-page.component.html`
- `frontend/src/app/features/trainings/pages/training-create-page.component.css`
- `frontend/src/app/features/trainings/pages/training-create-page.component.spec.ts`

**Responsibilities**
- `frontend/src/styles.css`: define the Executive Signal tokens, shared surfaces, table/forms primitives, and the light theme baseline for Angular Material.
- `docs/frontend/design-system.md`: replace the old dark-green contract with the new teal-blue, light, premium UI rules.
- `professor-landing-page.*`: implement the four-act landing approved in the spec.
- `login-page.*`: align auth with the new brand language without adding friction.
- `app-shell.*`: create the premium light workspace frame that the private product inherits.
- `dashboard-page.*`: improve operational prioritization and reduce card-equivalence.
- `student-dashboard-page.*`: make insights, planning, prova-alvo, and metrics easier to scan and reason about.
- `students` and `trainings` pages: upgrade the operational flows to task-first layouts with clearer actions and calmer surfaces.

### Task 1: Establish the Executive Signal foundation and redesign the landing page

**Files:**
- Modify: `frontend/src/index.html`
- Modify: `frontend/src/styles.css`
- Modify: `docs/frontend/design-system.md`
- Modify: `frontend/src/app/features/professor/pages/professor-landing-page.component.html`
- Modify: `frontend/src/app/features/professor/pages/professor-landing-page.component.css`
- Test: `frontend/src/app/features/professor/pages/professor-landing-page.component.spec.ts`

- [ ] **Step 1: Write the failing landing test for the new Executive Signal information architecture**

```ts
// frontend/src/app/features/professor/pages/professor-landing-page.component.spec.ts
it('renderiza a estrutura Executive Signal com prova de produto e fluxo do treinador', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Workspace premium para treinadores');
  expect(text).toContain('Sinais que importam');
  expect(text).toContain('Fluxo do treinador');
  expect(text).toContain('Criar conta de professor');
});
```

- [ ] **Step 2: Run the targeted test to verify it fails**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/professor/pages/professor-landing-page.component.spec.ts
```

Expected: FAIL because the current landing still renders the old hero, benefit cards, and signup intro.

- [ ] **Step 3: Replace the global tokens and landing structure with the new light premium system**

```html
<!-- frontend/src/index.html -->
<!doctype html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8">
  <title>CoachTraining | Performance Intelligence para Treinadores</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="theme-color" content="#163d57">
  <meta
    name="description"
    content="CoachTraining centraliza sinais de risco, taper, aderencia e historico de treino para treinadores de corrida tomarem decisoes com mais confianca.">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
</head>
<body>
  <app-root></app-root>
</body>
</html>
```

```css
/* frontend/src/styles.css */
@import url('https://fonts.googleapis.com/css2?family=Manrope:wght@500;600;700;800&family=Public+Sans:wght@400;500;600;700&display=swap');

:root {
  --font-family-display: 'Manrope', 'Segoe UI', sans-serif;
  --font-family-base: 'Public Sans', 'Segoe UI', sans-serif;

  --color-brand-950: #0f2737;
  --color-brand-900: #163d57;
  --color-brand-800: #1d566f;
  --color-brand-700: #2c7188;
  --color-brand-100: #d8e9f0;
  --color-brand-50: #edf5f8;

  --color-accent-700: #247b74;
  --color-accent-500: #34a29b;
  --color-accent-100: #d5f1ed;

  --color-bg: #eff4f6;
  --color-bg-secondary: #f7fafb;
  --color-surface: #ffffff;
  --color-surface-muted: #f2f6f8;
  --color-border: #d7e1e6;
  --color-text: #16354a;
  --color-text-muted: #5f7382;

  --color-success: #247b74;
  --color-warning: #b7791f;
  --color-error: #b54747;
  --color-info: #2c7188;

  --radius-md: 16px;
  --radius-lg: 24px;
  --radius-xl: 32px;
  --shadow-elevated: 0 24px 60px rgb(17 33 47 / 14%);
}

body {
  font-family: var(--font-family-base);
  color: var(--color-text);
  background:
    radial-gradient(circle at top left, rgb(44 113 136 / 8%), transparent 28%),
    linear-gradient(180deg, #f7fafb 0%, #eff4f6 100%);
}

h1, h2, h3, h4 {
  font-family: var(--font-family-display);
  letter-spacing: -0.03em;
  color: var(--color-brand-950);
}

.page-surface {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: rgb(255 255 255 / 78%);
  box-shadow: var(--shadow-elevated);
  backdrop-filter: blur(10px);
}

.section-label {
  color: var(--color-accent-700);
  text-transform: uppercase;
  letter-spacing: 0.16em;
  font-size: 0.78rem;
  font-weight: 700;
}
```

```html
<!-- frontend/src/app/features/professor/pages/professor-landing-page.component.html -->
<div class="landing-page">
  <header class="landing-topbar">
    <div class="brand-block">
      <p class="brand">CoachTraining</p>
      <p class="brand-subtitle">Performance intelligence for coaches</p>
    </div>
    <div class="topbar-actions">
      <a mat-button routerLink="/login">Entrar</a>
      <a mat-flat-button href="#cadastro">Cadastrar professor</a>
    </div>
  </header>

  <section class="hero page-surface" aria-label="Apresentacao do produto">
    <div class="hero-copy">
      <p class="section-label">Workspace premium para treinadores</p>
      <h1>Controle o ciclo do atleta com leitura precisa e decisao mais confiavel.</h1>
      <p class="hero-description">
        Organize atletas, acompanhe risco, taper e aderencia, e transforme sinais de treino em acoes mais claras para a proxima prova.
      </p>
      <div class="hero-actions">
        <a mat-flat-button href="#cadastro">Comecar cadastro</a>
        <a mat-button class="secondary-link" href="#produto">Ver como funciona</a>
      </div>
      <div class="hero-proof">
        <article><span>Dashboard individual</span><strong>12 semanas</strong></article>
        <article><span>Leituras-chave</span><strong>ACWR, taper, aderencia</strong></article>
        <article><span>Fluxo operacional</span><strong>Professor, atleta, treino</strong></article>
      </div>
    </div>

    <div class="hero-visual">
      <div class="hero-photo"></div>
      <aside class="hero-product page-surface">
        <p class="section-label">Resumo operacional</p>
        <h2>Prioridades do workspace</h2>
        <div class="mini-metrics">
          <article><span>Ativos</span><strong>24</strong></article>
          <article><span>Risco</span><strong>03</strong></article>
          <article><span>Taper</span><strong>02</strong></article>
        </div>
      </aside>
    </div>
  </section>

  <section id="produto" class="signal-strip">
    <div>
      <p class="section-label">Sinais que importam</p>
      <h2>Menos ruido operacional. Mais leitura do que realmente pede acao.</h2>
    </div>
    <div class="signal-list">
      <article><h3>Risco e progressao</h3><p>ACWR e delta semanal em leitura mais rapida.</p></article>
      <article><h3>Taper e prova-alvo</h3><p>Contexto competitivo integrado ao dashboard.</p></article>
      <article><h3>Aderencia ao plano</h3><p>Planejado versus realizado sem precisar montar planilhas paralelas.</p></article>
    </div>
  </section>

  <section class="trainer-flow">
    <div>
      <p class="section-label">Fluxo do treinador</p>
      <h2>Do cadastro ao ajuste da semana, o produto acompanha a rotina real.</h2>
    </div>
    <ol>
      <li>Cadastrar atleta</li>
      <li>Registrar sessoes</li>
      <li>Ler sinais de risco e aderencia</li>
      <li>Tomar a proxima decisao com contexto</li>
    </ol>
  </section>

  <section id="cadastro" class="signup-section page-surface">
    <div class="signup-copy">
      <p class="section-label">Criar conta de professor</p>
      <h2>Abra seu workspace e comece a acompanhar os atletas em um unico sistema.</h2>
    </div>

    <form [formGroup]="cadastroForm" (ngSubmit)="onSubmit()" class="signup-form" novalidate>
      <mat-form-field appearance="outline">
        <mat-label>Nome</mat-label>
        <input matInput formControlName="nome" autocomplete="name" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Email</mat-label>
        <input matInput type="email" formControlName="email" autocomplete="email" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Senha</mat-label>
        <input matInput type="password" formControlName="senha" autocomplete="new-password" />
      </mat-form-field>

      <div class="signup-actions">
        <button mat-flat-button type="submit" [disabled]="carregandoCadastro">
          {{ carregandoCadastro ? 'Enviando...' : 'Cadastrar professor' }}
        </button>
      </div>
    </form>
  </section>
</div>
```

- [ ] **Step 4: Update the design system doc so the new tokens become the official contract**

```md
<!-- docs/frontend/design-system.md -->
## Tema base

- Tema principal: light premium
- Cor estrutural: azul-petroleo
- Cor de apoio: verde funcional
- Layout: editorial, claro, com foco em leitura de prioridade e decisao

## Tipografia

- Display: `Manrope`
- Interface e conteudo: `Public Sans`

## Governanca

- Evitar cards por padrao.
- Usar superficie apenas quando ajudar a separar contexto, prioridade ou acao.
- CTA primario unico por tela; secundarios com menor peso visual.
```

- [ ] **Step 5: Run the targeted landing test and a build**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/professor/pages/professor-landing-page.component.spec.ts
npm run build
```

Expected:
- the landing spec PASSes
- the production build PASSes with the new global theme and landing markup

- [ ] **Step 6: Commit the foundation and landing redesign**

```bash
git add frontend/src/index.html frontend/src/styles.css docs/frontend/design-system.md frontend/src/app/features/professor/pages/professor-landing-page.component.html frontend/src/app/features/professor/pages/professor-landing-page.component.css frontend/src/app/features/professor/pages/professor-landing-page.component.spec.ts
git commit -m "feat: add executive signal landing and theme foundations"
```

### Task 2: Redesign the login flow to match the public brand language

**Files:**
- Modify: `frontend/src/app/features/auth/pages/login-page.component.html`
- Modify: `frontend/src/app/features/auth/pages/login-page.component.css`
- Test: `frontend/src/app/features/auth/pages/login-page.component.spec.ts`

- [ ] **Step 1: Write the failing login test for the new auth hierarchy**

```ts
// frontend/src/app/features/auth/pages/login-page.component.spec.ts
it('renderiza a nova hierarquia editorial do login', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Acesse o workspace do treinador');
  expect(text).toContain('Leituras de risco, taper e aderencia em um unico lugar');
  expect(text).toContain('Ainda nao tem conta? Criar acesso');
});
```

- [ ] **Step 2: Run the targeted login test to confirm it fails**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/auth/pages/login-page.component.spec.ts
```

Expected: FAIL because the current login still renders the old central dark card and copy.

- [ ] **Step 3: Implement the light premium auth layout**

```html
<!-- frontend/src/app/features/auth/pages/login-page.component.html -->
<section class="login-page" aria-label="Acesso de professor">
  <div class="login-shell page-surface">
    <div class="login-intro">
      <p class="section-label">CoachTraining</p>
      <h1>Acesse o workspace do treinador.</h1>
      <p class="description">Leituras de risco, taper e aderencia em um unico lugar, com a mesma linguagem visual da landing.</p>
      <a class="register-link" routerLink="/">Ainda nao tem conta? Criar acesso</a>
    </div>

    <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="login-form" novalidate>
      <mat-form-field appearance="outline">
        <mat-label>Email</mat-label>
        <input matInput type="email" formControlName="email" autocomplete="email" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Senha</mat-label>
        <input matInput type="password" formControlName="senha" autocomplete="current-password" />
      </mat-form-field>

      <button mat-flat-button type="submit" [disabled]="carregandoLogin">
        {{ carregandoLogin ? 'Entrando...' : 'Entrar' }}
      </button>
    </form>
  </div>
</section>
```

```css
/* frontend/src/app/features/auth/pages/login-page.component.css */
.login-page {
  min-height: 100dvh;
  display: grid;
  place-items: center;
  padding: 2rem 1.25rem;
}

.login-shell {
  width: min(980px, 100%);
  display: grid;
  grid-template-columns: minmax(0, 0.9fr) minmax(340px, 0.75fr);
  gap: 1.5rem;
  padding: 1.5rem;
}

.login-intro {
  display: grid;
  align-content: center;
  gap: 1rem;
  padding: 1rem;
}

.login-form {
  display: grid;
  gap: 1rem;
  padding: 1.25rem;
  border-radius: var(--radius-lg);
  background: var(--color-surface-muted);
  border: 1px solid var(--color-border);
}
```

- [ ] **Step 4: Run the targeted login test**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/auth/pages/login-page.component.spec.ts
```

Expected: PASS with the new copy and layout.

- [ ] **Step 5: Commit the login redesign**

```bash
git add frontend/src/app/features/auth/pages/login-page.component.html frontend/src/app/features/auth/pages/login-page.component.css frontend/src/app/features/auth/pages/login-page.component.spec.ts
git commit -m "feat: redesign login flow with executive signal"
```

### Task 3: Modernize the authenticated shell and the professor home

**Files:**
- Create: `frontend/src/app/core/layout/app-shell.component.spec.ts`
- Modify: `frontend/src/app/core/layout/app-shell.component.ts`
- Modify: `frontend/src/app/core/layout/app-shell.component.html`
- Modify: `frontend/src/app/core/layout/app-shell.component.css`
- Modify: `frontend/src/app/features/dashboard/pages/dashboard-page.component.html`
- Modify: `frontend/src/app/features/dashboard/pages/dashboard-page.component.css`
- Test: `frontend/src/app/features/dashboard/pages/dashboard-page.component.spec.ts`

- [ ] **Step 1: Write the failing shell and dashboard tests**

```ts
// frontend/src/app/core/layout/app-shell.component.spec.ts
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { AppShellComponent } from './app-shell.component';
import { AuthService } from '../../services/auth/auth.service';

describe('AppShellComponent', () => {
  let fixture: ComponentFixture<AppShellComponent>;
  let component: AppShellComponent;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['logout']);

    await TestBed.configureTestingModule({
      imports: [AppShellComponent],
      providers: [{ provide: AuthService, useValue: authService }, provideRouter([])]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigateByUrl').and.resolveTo(true);
    fixture = TestBed.createComponent(AppShellComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renderiza o shell com brand, contexto e navegacao principal', () => {
    const text = fixture.nativeElement.textContent;
    expect(text).toContain('CoachTraining');
    expect(text).toContain('Workspace do professor');
    expect(text).toContain('Leituras');
    expect(text).toContain('Novo treino');
  });
});
```

```ts
// frontend/src/app/features/dashboard/pages/dashboard-page.component.spec.ts
it('renderiza a nova home com foco em leituras do workspace', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Leituras do workspace');
  expect(text).toContain('Atletas prioritarios');
  expect(text).toContain('Treinos recentes');
});
```

- [ ] **Step 2: Run the targeted tests to verify they fail**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/core/layout/app-shell.component.spec.ts --include src/app/features/dashboard/pages/dashboard-page.component.spec.ts
```

Expected: FAIL because the shell spec is new and the professor dashboard does not render the new section copy yet.

- [ ] **Step 3: Refactor the shell into a light premium workspace frame**

```ts
// frontend/src/app/core/layout/app-shell.component.ts
readonly navItems = [
  { label: 'Leituras', link: '/dashboard', exact: true },
  { label: 'Alunos', link: '/dashboard/alunos', exact: true },
  { label: 'Novo aluno', link: '/dashboard/alunos/novo', exact: true },
  { label: 'Novo treino', link: '/dashboard/treinos/novo', exact: true }
];
```

```html
<!-- frontend/src/app/core/layout/app-shell.component.html -->
<div class="shell-layout">
  <aside class="shell-sidebar" [class.open]="sidebarAberta">
    <div class="sidebar-brand">
      <p class="brand">CoachTraining</p>
      <p class="context">Workspace do professor</p>
    </div>

    <nav class="sidebar-nav" aria-label="Navegacao principal">
      @for (item of navItems; track item.link) {
        <a
          [routerLink]="item.link"
          routerLinkActive="active"
          [routerLinkActiveOptions]="{ exact: item.exact }"
          (click)="fecharSidebar()">
          {{ item.label }}
        </a>
      }
    </nav>

    <button mat-stroked-button type="button" (click)="logout()">Sair</button>
  </aside>

  <div class="shell-content">
    <header class="shell-header">
      <button class="menu-button" type="button" (click)="toggleSidebar()" aria-label="Abrir menu">☰</button>
      <div>
        <p class="section-label">CoachTraining</p>
        <h1>Leituras do workspace</h1>
      </div>
    </header>

    <main class="shell-main" (click)="fecharSidebar()">
      <router-outlet></router-outlet>
    </main>
  </div>
</div>
```

```css
/* frontend/src/app/core/layout/app-shell.component.css */
.shell-layout {
  min-height: 100dvh;
  display: grid;
  grid-template-columns: 280px minmax(0, 1fr);
  background: var(--color-bg);
}

.shell-sidebar {
  display: grid;
  align-content: start;
  gap: 1.25rem;
  padding: 1.25rem;
  background: linear-gradient(180deg, #13364b 0%, #173d55 100%);
  color: white;
}

.shell-content {
  display: grid;
  grid-template-rows: auto 1fr;
}

.shell-header {
  padding: 1.25rem 1.5rem 0;
}

.shell-main {
  padding: 1.5rem;
}
```

- [ ] **Step 4: Redesign the professor dashboard around priority, not card parity**

```html
<!-- frontend/src/app/features/dashboard/pages/dashboard-page.component.html -->
<section class="dashboard" aria-label="Tela inicial do professor">
  <header class="dashboard-hero page-surface">
    <div>
      <p class="section-label">Leituras do workspace</p>
      <h1>Resumo operacional dos atletas.</h1>
      <p>Acompanhe prioridades, ultimos registros e acoes centrais do dia sem navegar por blocos equivalentes.</p>
    </div>
    <div class="hero-actions">
      <a mat-flat-button routerLink="/dashboard/alunos/novo">Cadastrar aluno</a>
      <a mat-button routerLink="/dashboard/treinos/novo">Registrar treino</a>
    </div>
  </header>

  <div class="kpi-strip">
    <article class="page-surface">
      <span>Atletas ativos</span>
      <strong>{{ resumo?.totalAtletas }}</strong>
      <small>{{ resumo?.atletasComPlanejamentoConfigurado }} com planejamento configurado</small>
    </article>
    <article class="page-surface">
      <span>Atencao ou risco</span>
      <strong>{{ resumo?.atletasEmAtencao }}</strong>
      <small>{{ resumo?.atletasEmRisco }} em risco alto</small>
    </article>
    <article class="page-surface">
      <span>Treinos na semana</span>
      <strong>{{ resumo?.treinosRegistradosNaSemana }}</strong>
      <small>Janela movel dos ultimos 7 dias</small>
    </article>
    <article class="page-surface">
      <span>Em taper</span>
      <strong>{{ resumo?.atletasEmTaper }}</strong>
      <small>Atletas com prova na janela de polimento</small>
    </article>
  </div>

  <div class="content-grid">
    <section class="page-surface">
      <div class="panel-header">
        <div>
          <p class="section-label">Prioridade</p>
          <h2>Atletas prioritarios</h2>
        </div>
      </div>
    </section>

    <section class="page-surface">
      <div class="panel-header">
        <div>
          <p class="section-label">Agenda de acompanhamento</p>
          <h2>Treinos recentes</h2>
        </div>
      </div>
    </section>
  </div>
</section>
```

- [ ] **Step 5: Run the targeted shell and professor dashboard tests**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/core/layout/app-shell.component.spec.ts --include src/app/features/dashboard/pages/dashboard-page.component.spec.ts
```

Expected: PASS with the new shell and the updated professor home hierarchy.

- [ ] **Step 6: Commit the shell and professor dashboard redesign**

```bash
git add frontend/src/app/core/layout/app-shell.component.ts frontend/src/app/core/layout/app-shell.component.html frontend/src/app/core/layout/app-shell.component.css frontend/src/app/core/layout/app-shell.component.spec.ts frontend/src/app/features/dashboard/pages/dashboard-page.component.html frontend/src/app/features/dashboard/pages/dashboard-page.component.css frontend/src/app/features/dashboard/pages/dashboard-page.component.spec.ts
git commit -m "feat: modernize workspace shell and professor dashboard"
```

### Task 4: Redesign the athlete dashboard around decision-making clarity

**Files:**
- Modify: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.html`
- Modify: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.css`
- Test: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.spec.ts`

- [ ] **Step 1: Write the failing athlete dashboard test**

```ts
// frontend/src/app/features/dashboard/pages/student-dashboard-page.component.spec.ts
it('renderiza a nova hierarquia com leitura de performance e contexto competitivo', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Leitura de performance');
  expect(text).toContain('Planejamento e contexto competitivo');
  expect(text).toContain('Insights prioritarios');
});
```

- [ ] **Step 2: Run the targeted athlete dashboard test to confirm the failure**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/dashboard/pages/student-dashboard-page.component.spec.ts
```

Expected: FAIL because the current dashboard still renders separate flat cards and old section headings.

- [ ] **Step 3: Reorganize the dashboard into summary, planning context, charts, and insights**

```html
<!-- frontend/src/app/features/dashboard/pages/student-dashboard-page.component.html -->
<section class="student-dashboard" aria-label="Dashboard do aluno">
  <header class="dashboard-header page-surface">
    <div>
      <p class="section-label">Leitura de performance</p>
      <h1>{{ dashboard?.nome }}</h1>
      <p class="subtext">Ultima atualizacao: {{ dashboard?.dataUltimaAtualizacao | date: 'dd/MM/yyyy HH:mm' }}</p>
    </div>
    <div class="actions">
      <button mat-button type="button" (click)="exportarExcel()" [disabled]="!podeExportar">Exportar Excel</button>
      <button mat-flat-button type="button" (click)="exportarPdf()" [disabled]="!podeExportar">Exportar PDF</button>
    </div>
  </header>

  <section class="summary-grid">
    <article class="page-surface">
      <span>Carga semanal</span>
      <strong>{{ dashboard?.cargaSemanal }}</strong>
      <small>Delta: {{ dashboard?.deltaPercentualSemanal | number: '1.0-1' }}%</small>
    </article>
    <article class="page-surface">
      <span>Carga cronica</span>
      <strong>{{ dashboard?.cargaCronica }}</strong>
      <small>Carga aguda: {{ dashboard?.cargaAguda }}</small>
    </article>
    <article class="page-surface">
      <span>ACWR</span>
      <strong>{{ dashboard?.acwr | number: '1.2-2' }}</strong>
      <small [class]="statusRiscoClasse">Risco: {{ statusRiscoDescricao }}</small>
    </article>
    <article class="page-surface">
      <span>Fase</span>
      <strong>{{ faseDescricao }}</strong>
      <small *ngIf="dashboard?.proximaProva">Prova: {{ formatarData(dashboard?.proximaProva) }}</small>
    </article>
  </section>

  <section class="planning-grid">
    <article class="page-surface">
      <p class="section-label">Planejamento e contexto competitivo</p>
      <h2>Prova alvo e planejamento base</h2>
      <form class="target-form" [formGroup]="provaAlvoForm">
        <mat-form-field appearance="outline">
          <mat-label>Data da prova</mat-label>
          <input matInput type="date" formControlName="dataProva" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Distancia (km)</mat-label>
          <input matInput type="number" formControlName="distanciaKm" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Objetivo</mat-label>
          <input matInput type="text" formControlName="objetivo" />
        </mat-form-field>
      </form>

      <form class="planning-form" [formGroup]="planejamentoBaseForm">
        <mat-form-field appearance="outline">
          <mat-label>Treinos planejados por semana</mat-label>
          <input matInput type="number" formControlName="treinosPlanejadosPorSemana" />
        </mat-form-field>
      </form>
    </article>

    <article class="page-surface insight-surface">
      <p class="section-label">Insights prioritarios</p>
      <h2>O que pede atencao agora</h2>
      <ul *ngIf="dashboard?.insights?.length">
        <li *ngFor="let insight of dashboard?.insights">{{ insight }}</li>
      </ul>
    </article>
  </section>

  <section class="charts-grid">
    <article class="page-surface">
      <h2>Carga semanal</h2>
      <div class="chart-container">
        <canvas #cargaChart aria-label="Grafico de carga semanal"></canvas>
      </div>
    </article>
    <article class="page-surface">
      <h2>Pace medio semanal</h2>
      <div class="chart-container">
        <canvas #paceChart aria-label="Grafico de pace semanal"></canvas>
      </div>
    </article>
  </section>

  <section class="page-surface">
    <p class="section-label">Historico operacional</p>
    <h2>Treinos da janela</h2>
    <div class="table-wrapper">
      <table>
        <thead>
          <tr>
            <th>Data</th>
            <th>Tipo</th>
            <th>Duracao</th>
            <th>Distancia</th>
            <th>RPE</th>
            <th>Carga</th>
            <th>Pace</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let treino of dashboard?.treinosJanela">
            <td>{{ formatarData(treino.data) }}</td>
            <td>{{ obterTipoTreinoDescricao(treino.tipo) }}</td>
            <td>{{ treino.duracaoMinutos }} min</td>
            <td>{{ treino.distanciaKm | number: '1.1-1' }} km</td>
            <td>{{ treino.rpe }}</td>
            <td>{{ treino.carga }}</td>
            <td>{{ treino.paceMinPorKm != null ? (treino.paceMinPorKm | number: '1.2-2') : '-' }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</section>
```

```css
/* frontend/src/app/features/dashboard/pages/student-dashboard-page.component.css */
.dashboard-header,
.planning-grid > article,
.charts-grid > article,
.summary-grid article {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1rem;
}

.planning-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.2fr) minmax(320px, 0.8fr);
  gap: 1rem;
}

.insight-surface ul {
  margin-top: 1rem;
}
```

- [ ] **Step 4: Run the targeted athlete dashboard test**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/dashboard/pages/student-dashboard-page.component.spec.ts
```

Expected: PASS with the new headings and grouped decision layout.

- [ ] **Step 5: Commit the athlete dashboard redesign**

```bash
git add frontend/src/app/features/dashboard/pages/student-dashboard-page.component.html frontend/src/app/features/dashboard/pages/student-dashboard-page.component.css frontend/src/app/features/dashboard/pages/student-dashboard-page.component.spec.ts
git commit -m "feat: redesign athlete dashboard hierarchy"
```

### Task 5: Modernize students list and the create flows

**Files:**
- Modify: `frontend/src/app/features/students/pages/students-list-page.component.html`
- Modify: `frontend/src/app/features/students/pages/students-list-page.component.css`
- Modify: `frontend/src/app/features/students/pages/students-list-page.component.spec.ts`
- Modify: `frontend/src/app/features/students/pages/student-create-page.component.html`
- Modify: `frontend/src/app/features/students/pages/student-create-page.component.css`
- Modify: `frontend/src/app/features/students/pages/student-create-page.component.spec.ts`
- Modify: `frontend/src/app/features/trainings/pages/training-create-page.component.html`
- Modify: `frontend/src/app/features/trainings/pages/training-create-page.component.css`
- Modify: `frontend/src/app/features/trainings/pages/training-create-page.component.spec.ts`

- [ ] **Step 1: Write the failing tests for the new task-first layouts**

```ts
// frontend/src/app/features/students/pages/students-list-page.component.spec.ts
it('renderiza a lista como workspace de atletas', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Atletas do workspace');
  expect(text).toContain('Lista operacional');
});
```

```ts
// frontend/src/app/features/students/pages/student-create-page.component.spec.ts
it('renderiza secoes de identidade e contexto tecnico no cadastro', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Identidade do atleta');
  expect(text).toContain('Contexto tecnico');
});
```

```ts
// frontend/src/app/features/trainings/pages/training-create-page.component.spec.ts
it('renderiza o formulario em blocos de selecao, sessao e esforco', () => {
  fixture.detectChanges();

  const text = fixture.nativeElement.textContent;
  expect(text).toContain('Selecionar atleta');
  expect(text).toContain('Detalhes da sessao');
  expect(text).toContain('Esforco percebido');
});
```

- [ ] **Step 2: Run the targeted flow tests to confirm they fail**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/students/pages/students-list-page.component.spec.ts --include src/app/features/students/pages/student-create-page.component.spec.ts --include src/app/features/trainings/pages/training-create-page.component.spec.ts
```

Expected: FAIL because the current screens still use the old sprint-era wording and raw form layout.

- [ ] **Step 3: Implement the students workspace and the guided forms**

```html
<!-- frontend/src/app/features/students/pages/students-list-page.component.html -->
<section class="students" aria-label="Listagem de alunos">
  <header class="page-surface">
    <div>
      <p class="section-label">Lista operacional</p>
      <h1>Atletas do workspace</h1>
      <p>Visualize rapidamente quem ja esta ativo e siga para o dashboard individual sem ruido visual.</p>
    </div>
    <a mat-flat-button routerLink="/dashboard/alunos/novo">Novo aluno</a>
  </header>

  <div *ngIf="!carregando && alunos.length === 0" class="empty-state page-surface">
    <h2>Nenhum atleta cadastrado ainda.</h2>
    <p>Comece adicionando o primeiro atleta para liberar dashboards individuais e registros de treino.</p>
    <a mat-flat-button routerLink="/dashboard/alunos/novo">Cadastrar atleta</a>
  </div>

  <div *ngIf="!carregando && alunos.length > 0" class="table-wrapper page-surface">
    <table>
      <thead>
        <tr>
          <th>Nome</th>
          <th>Email</th>
          <th>Nivel</th>
          <th>Observacoes clinicas</th>
          <th>Data de cadastro</th>
          <th>Acao</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let aluno of alunos">
          <td>{{ aluno.nome }}</td>
          <td>{{ aluno.email || '-' }}</td>
          <td>{{ aluno.nivelEsportivo || '-' }}</td>
          <td>{{ aluno.observacoesClinicas || '-' }}</td>
          <td>{{ formatarData(aluno.dataCriacao) }}</td>
          <td><a mat-button [routerLink]="['/dashboard/alunos', aluno.id]">Abrir dashboard</a></td>
        </tr>
      </tbody>
    </table>
  </div>
</section>
```

```html
<!-- frontend/src/app/features/students/pages/student-create-page.component.html -->
<section class="student-create" aria-label="Cadastro de aluno">
  <header class="page-surface">
    <p class="section-label">Novo atleta</p>
    <h1>Cadastrar atleta</h1>
    <p>Preencha primeiro a identidade do atleta e depois o contexto tecnico que ajuda a leitura do dashboard.</p>
  </header>

  <form [formGroup]="form" (ngSubmit)="salvar()" novalidate class="create-form page-surface">
    <section>
      <p class="section-label">Identidade do atleta</p>
      <mat-form-field appearance="outline">
        <mat-label>Nome</mat-label>
        <input matInput formControlName="nome" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Email (opcional)</mat-label>
        <input matInput formControlName="email" />
      </mat-form-field>
    </section>

    <section>
      <p class="section-label">Contexto tecnico</p>
      <mat-form-field appearance="outline">
        <mat-label>Nivel esportivo</mat-label>
        <input matInput formControlName="nivelEsportivo" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Observacoes clinicas</mat-label>
        <textarea matInput rows="4" formControlName="observacoesClinicas"></textarea>
      </mat-form-field>
    </section>
  </form>
</section>
```

```html
<!-- frontend/src/app/features/trainings/pages/training-create-page.component.html -->
<section class="training-create-page">
  <header class="page-surface">
    <p class="section-label">Novo treino</p>
    <h1>Registrar sessao de treino</h1>
    <p>Selecione o atleta, descreva a sessao e informe o esforco percebido para manter a leitura de carga consistente.</p>
  </header>

  <form [formGroup]="form" (ngSubmit)="salvar()" class="training-form page-surface">
    <section>
      <p class="section-label">Selecionar atleta</p>
      <mat-form-field appearance="outline">
        <mat-label>Buscar aluno</mat-label>
        <input matInput formControlName="buscaAtleta" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Aluno</mat-label>
        <mat-select formControlName="atletaId"></mat-select>
      </mat-form-field>
    </section>

    <section>
      <p class="section-label">Detalhes da sessao</p>
      <mat-form-field appearance="outline">
        <mat-label>Data</mat-label>
        <input matInput type="date" formControlName="data" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Tipo de treino</mat-label>
        <mat-select formControlName="tipo"></mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Duracao (min)</mat-label>
        <input matInput type="number" formControlName="duracaoMinutos" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Distancia (km)</mat-label>
        <input matInput type="number" formControlName="distanciaKm" />
      </mat-form-field>
    </section>

    <section>
      <p class="section-label">Esforco percebido</p>
      <mat-form-field appearance="outline">
        <mat-label>RPE (1 a 10)</mat-label>
        <input matInput type="number" formControlName="rpe" />
        <mat-hint>Percepcao subjetiva de esforco: 1 = muito facil, 10 = exaustivo.</mat-hint>
      </mat-form-field>
    </section>
  </form>
</section>
```

- [ ] **Step 4: Run the targeted flow tests**

Run:

```bash
cd frontend
npx ng test --watch=false --browsers=ChromeHeadless --include src/app/features/students/pages/students-list-page.component.spec.ts --include src/app/features/students/pages/student-create-page.component.spec.ts --include src/app/features/trainings/pages/training-create-page.component.spec.ts
```

Expected: PASS with the new headings and grouped form layout.

- [ ] **Step 5: Commit the students and training flow redesign**

```bash
git add frontend/src/app/features/students/pages/students-list-page.component.html frontend/src/app/features/students/pages/students-list-page.component.css frontend/src/app/features/students/pages/students-list-page.component.spec.ts frontend/src/app/features/students/pages/student-create-page.component.html frontend/src/app/features/students/pages/student-create-page.component.css frontend/src/app/features/students/pages/student-create-page.component.spec.ts frontend/src/app/features/trainings/pages/training-create-page.component.html frontend/src/app/features/trainings/pages/training-create-page.component.css frontend/src/app/features/trainings/pages/training-create-page.component.spec.ts
git commit -m "feat: modernize student and training flows"
```

### Task 6: Final documentation sync and full frontend verification

**Files:**
- Modify: `docs/frontend/design-system.md`

- [ ] **Step 1: Add the final governance notes to the frontend design doc**

```md
<!-- docs/frontend/design-system.md -->
## Superficies operacionais

- Dashboards usam superficies para separar prioridade, nao para repetir card mosaics.
- Formularios usam secoes por intencao: identidade, contexto, sessao, esforco.
- Tabelas devem priorizar leitura e acao, com cabecalhos discretos e estados vazios orientadores.

## Landing e autenticacao

- Landing usa hero com fotografia + mock do produto.
- Login deve parecer parte do mesmo produto, com a mesma paleta e hierarquia.
```

- [ ] **Step 2: Run the full frontend verification suite**

Run:

```bash
cd frontend
npm test -- --watch=false --browsers=ChromeHeadless
npm run build
```

Expected:
- all frontend unit tests PASS
- production build PASSes

- [ ] **Step 3: Review the final worktree state**

Run:

```bash
git status --short
git log --oneline -6
```

Expected:
- `git status --short` shows a clean tree
- `git log --oneline -6` shows the focused redesign commits from this plan

- [ ] **Step 4: Commit the final documentation sync if needed**

```bash
git add docs/frontend/design-system.md
git commit -m "docs: finalize executive signal frontend guidelines"
```
